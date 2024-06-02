// based on original from Microsoft:
//    https://github.com/mganss/XmlSchemaClassGenerator

using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

// -----------------------------------------------------------------------------
using Edam.Xml.XmlAsset;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;

namespace Edam.Xml.XmlExplore
{

   //To build substitutionGroups
   internal class SubstitutionGroupWrapper
   {
      XmlQualifiedName head;
      ArrayList members = new ArrayList();

      internal XmlQualifiedName Head
      {
         get
         {
            return head;
         }
         set
         {
            head = value;
         }
      }

      internal ArrayList Members
      {
         get
         {
            return members;
         }
      }
   }

   internal class AssetGroupItem
   {
      public ElementType ElementType { get; set; }
      public ElementGroup GroupType { get; set; }
      public XmlSchemaObject Item { get; set; }

      public AssetGroupItem(
         ElementType type, ElementGroup group, XmlSchemaObject item)
      {
         ElementType = type;
         GroupType = group;
         Item = item;
      }
   }

   /// <summary>
   /// Xml Tag Support
   /// </summary>
   public class XmlTagCounter
   {
      public string TagName { get; set; }
      public int Count { get; set; }

      public XmlTagCounter(string name)
      {
         TagName = name;
         Count = 0;
      }
   }

   public class XmlTagInspector
   {
      public bool PreventCycling { get; set; }
      public int MaxCount { get; set; }
      public Dictionary<string, XmlTagCounter> m_Items { get; set; }
      public XmlTagCounter CurrentTag { get; set; }

      public XmlTagInspector()
      {
         m_Items = new Dictionary<string, XmlTagCounter>();
         PreventCycling = false;
         MaxCount = 1;
         CurrentTag = null;
      }

      public void Clear()
      {
         m_Items.Clear();
         CurrentTag = null;
      }

      public XmlTagCounter Find(string name)
      {
         XmlTagCounter tag;
         if (!m_Items.TryGetValue(name, out tag))
         {
            tag = new XmlTagCounter(name);
            m_Items.Add(name, tag);
         }

         CurrentTag = tag;
         return tag;
      }

      public XmlTagCounter Count(string name)
      {
         XmlTagCounter tag = Find(name);
         tag.Count++;
         return tag;
      }

      public bool Inspect(string name)
      {
         var t = CurrentTag != null && CurrentTag.TagName == name ?
            CurrentTag : Find(name);
         return t.Count < MaxCount;
      }

      public void ShowCounts()
      {
         foreach(var i in m_Items)
         {
            System.Console.WriteLine("Name: " + i.Key
               + " Count: " + i.Value.Count.ToString());
         }
      }
   }

   /// <summary>
   /// Xml Crawler
   /// </summary>
   public class XmlCrawler
   {

      #region -- Fields and Properties declarations

      private static readonly string DEFAULT_OCCUR = "1";

      private XmlSchemaSet schemaSet;
      private XmlWriter writer;
      private XmlResolver xmlResolver;
      private InstanceElement instanceRoot;
      private XmlQualifiedName rootElement;
      private string rootTargetNamespace;

      internal const string NsXsd = "http://www.w3.org/2001/XMLSchema";
      internal const string NsXsi = "http://www.w3.org/2001/XMLSchema-instance";
      internal const string NsXml = "http://www.w3.org/XML/1998/namespace";

      internal XmlQualifiedName QnXsdAnyType =
         new XmlQualifiedName("anyType", NsXsd);
      internal XmlQualifiedName XsdNil = new XmlQualifiedName("nil", NsXsi);

      //Default options
      private int maxThreshold = 1;
      private int listLength = 1;

      private readonly XmlTagInspector m_TagInspector = new XmlTagInspector();

      //To pick-up recursive element defs
      private Hashtable elementTypesProcessed;
      private Hashtable instanceElementsProcessed;

      //Handle substitutionGroups
      private Hashtable substitutionGroupsTable;

      private XmlSchemaType AnyType;

      public int MaxThreshold
      {
         get
         {
            return maxThreshold;
         }
         set
         {
            maxThreshold = value;
         }
      }

      public int ListLength
      {
         get
         {
            return listLength;
         }
         set
         {
            listLength = value;
         }
      }

      public XmlResolver XmlResolver
      {
         set
         {
            xmlResolver = value;
         }
      }

      private XmlDataAsset _Asset = null;
      private Action<InstanceElement> _ProcessInstanceTree;
      private Action<InstanceElement> _ProcessElement;

      #endregion
      #region -- XML Sample Generator Constructors

      public XmlCrawler(string url, XmlQualifiedName rootElem) :
         this(XmlSchema.Read(new XmlTextReader(url), 
            new ValidationEventHandler(ValidationCallBack)), rootElem)
      {
      }

      public XmlCrawler(XmlSchema schema, XmlQualifiedName rootElem)
      {
         if (schema == null)
         {
            throw new Exception("Provided Schema is null. Xml cannot be generated.");
         }
         rootElement = rootElem;
         schemaSet = new XmlSchemaSet();
         schemaSet.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
         if (xmlResolver == null)
         {
            xmlResolver = new XmlUrlResolver();
         }
         schemaSet.XmlResolver = xmlResolver;
         schemaSet.Add(schema);
         AnyType = XmlSchemaType.GetBuiltInComplexType(XmlTypeCode.Item);
      }

      public XmlCrawler(XmlSchemaSet schemaSet, XmlQualifiedName rootElem)
      {
         if (schemaSet == null || schemaSet.Count == 0)
            throw new Exception("Provided Schema set is empty. Xml cannot be generated.");
         this.schemaSet = schemaSet;
         schemaSet.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
         if (xmlResolver == null)
         {
            xmlResolver = new XmlUrlResolver();
         }
         schemaSet.XmlResolver = xmlResolver;
         rootElement = rootElem;
         AnyType = XmlSchemaType.GetBuiltInComplexType(XmlTypeCode.Item);
      }

      #endregion
      #region -- XML Writers support

      public void WriteXml(XmlWriter writer)
      {
         if (writer == null)
         {
            throw new ArgumentNullException("writer");
         }

         _ProcessInstanceTree = ProcessInstanceTree;
         _ProcessElement = ProcessElement;

         this.writer = writer;
         elementTypesProcessed = new Hashtable();
         if (ProcessSchemaSet())
         {
            //Only if valid schemas were loaded
            if (instanceRoot != null)
            {
               //If found a root to generate XML
               instanceElementsProcessed = new Hashtable();
               _ProcessInstanceTree(instanceRoot);
            }
            this.writer.Flush();
         }
      }

      public void WriteXmlAsset(XmlDataAsset assets)
      {
         if (assets == null)
         {
            throw new ArgumentNullException("asset");
         }

         _Asset = assets;
         _ProcessInstanceTree = AssetProcessInstanceTree;
         _ProcessElement = AssetProcessElement;

         assets.Namespaces =
            XmlSchemaInspector.SetNamespaces(
               assets.Namespaces, schemaSet, null);

         elementTypesProcessed = new Hashtable();
         if (ProcessSchemaSet())
         { 
            //Only if valid schemas were loaded
            if (instanceRoot != null)
            { 
               //If found a root to generate XML
               instanceElementsProcessed = new Hashtable();
               _ProcessInstanceTree(instanceRoot);
            }
         }
         //m_TagInspector.ShowCounts();
      }

      #endregion
      #region -- XML Validation support

      private static void ValidationCallBack(object sender, ValidationEventArgs args)
      {
         Console.WriteLine("Error in Schema - ");
         Console.WriteLine(args.Message);
      }

      #endregion
      #region -- XML Get artifacts support

      private XmlSchemaComplexType GetDerivedType(XmlSchemaType baseType)
      { //To get derived type of an abstract type for xsi:type value in the instance
         foreach (XmlSchemaType type in schemaSet.GlobalTypes.Values)
         {
            XmlSchemaComplexType ct = type as XmlSchemaComplexType;
            if (ct != null && !ct.IsAbstract && XmlSchemaType.IsDerivedFrom(ct, baseType, XmlSchemaDerivationMethod.None))
            {
               return ct;
            }
         }
         return null;
      }

      private XmlSchemaAttribute GetAttributeFromNS(string ns, XmlSchemaObjectTable attributes)
      {
         return GetAttributeFromNS(ns, false, attributes);
      }

      private XmlSchemaAttribute GetAttributeFromNS(string ns, bool other, XmlSchemaObjectTable attributes)
      {
         if (other)
         {
            foreach (XmlSchemaAttribute attr in schemaSet.GlobalAttributes.Values)
            {
               if (attr.QualifiedName.Namespace != ns && attr.QualifiedName.Namespace != string.Empty && attributes[attr.QualifiedName] == null)
               {
                  return attr;
               }
            }
         }
         else
         {
            foreach (XmlSchemaAttribute attr in schemaSet.GlobalAttributes.Values)
            {
               if (attr.QualifiedName.Namespace == ns && attributes[attr.QualifiedName] == null)
               {
                  return attr;
               }
            }
         }
         return null;
      }

      //For all of these methods, Shd i store the element the first time and reuse the same
      //instead of looking up the hashtable again?
      private XmlSchemaElement GetElementFromNS(string ns)
      {
         return GetElementFromNS(ns, false);
      }

      private XmlSchemaElement GetElementFromNS(string ns, bool other)
      {
         if (other)
         {
            foreach (XmlSchemaElement elem in schemaSet.GlobalElements.Values)
            {
               if (elem.QualifiedName.Namespace != ns && elem.QualifiedName.Namespace != string.Empty)
               {
                  return elem;
               }
            }
         }
         else
         {
            foreach (XmlSchemaElement elem in schemaSet.GlobalElements.Values)
            {
               if (elem.QualifiedName.Namespace == ns && !elem.QualifiedName.Equals(rootElement))
               {
                  return elem;
               }
            }
         }
         return null;
      }

      private XmlSchema GetParentSchema(XmlSchemaObject currentSchemaObject)
      {
         XmlSchema parentSchema = null;
         //Debug.Assert((currentSchemaObject as XmlSchema) == null); //The current object should not be schema
         while (parentSchema == null && currentSchemaObject != null)
         {
            currentSchemaObject = currentSchemaObject.Parent;
            parentSchema = currentSchemaObject as XmlSchema;
         }
         return parentSchema;
      }

      private InstanceElement GetParentInstanceElement(InstanceGroup grp)
      {
         InstanceElement elem = grp as InstanceElement;
         while (elem == null && grp != null)
         {
            grp = grp.Parent;
            elem = grp as InstanceElement;
         }
         return elem;
      }

      #endregion
      #region -- XML Processing Support

      private bool ProcessSchemaSet()
      {
         m_TagInspector.Clear();

         //Add all the Elements from all schemas into the Elements table
         schemaSet.Compile();
         if (schemaSet.IsCompiled)
         {
            XmlSchemaElement schemaElem = null;
            if (rootElement == null)
            {
               rootElement = XmlQualifiedName.Empty;
            }
            schemaElem = schemaSet.GlobalElements[rootElement] as XmlSchemaElement;
            if (schemaElem == null)
            { //If element by name is not found, Get first non-abstract root element
               foreach (XmlSchemaElement elem1 in schemaSet.GlobalElements.Values)
               {
                  if (elem1.IsAbstract)
                  {
                     continue;
                  }
                  schemaElem = elem1;
                  rootElement = schemaElem.QualifiedName;
                  break;
               }
            }
            if (schemaElem != null)
            {
               rootTargetNamespace = schemaElem.QualifiedName.Namespace;
               GenerateElement(schemaElem, true, null, null);
            }
            else
            { //No root element found
               Console.WriteLine("No root element was found, XML cannot be generated.");
            }
            return true;
         }
         return false;
      }

      private void ProcessComplexType(XmlSchemaComplexType ct, InstanceElement elem)
      {
         if (ct.ContentModel != null && ct.ContentModel is XmlSchemaSimpleContent)
         {
            elem.ValueGenerator = XmlValueGenerator.CreateGenerator(ct.Datatype, listLength);
         }
         else
         {
            GenerateParticle(ct.ContentTypeParticle, false, elem);
         }
         //Check for attribute wild card
         if (ct.AttributeWildcard != null)
         {
            GenerateAttributeWildCard(ct, elem);
         }
         //Check for attributes if simple/complex content
         if (ct.AttributeUses.Count > 0)
         {
            GenerateAttribute(ct.AttributeUses, elem);
         }
      }

      private void ProcessGroup(InstanceGroup grp)
      {
         if (grp is InstanceElement)
         {
            _ProcessElement((InstanceElement)grp);
         }
         else
         { //Its a group node of sequence or choice
            if (!grp.IsChoice)
            {
               for (int i = 0; i < grp.Occurs; i++)
               {
                  InstanceGroup childGroup = grp.Child;
                  while (childGroup != null)
                  {
                     ProcessGroup(childGroup);
                     childGroup = childGroup.Sibling;
                  }
               }
            }
            else
            {
               ProcessChoiceGroup(grp);
            }
         }
      }

      private void ProcessChoiceGroup(InstanceGroup grp)
      {
         if (_Asset == null)
         {
            for (int i = 0; i < grp.Occurs; i++)
            { //Cyclically iterate over the children of choice
               ProcessGroup(grp.GetChild(i % grp.NoOfChildren));
            }
            return;
         }

         // while preparing the assets dictionary show all individual choices
         InstanceGroup child = grp.Child;
         for (int i = 0; i < grp.NoOfChildren; i++)
         {
            ProcessGroup(child);
            child = child.Sibling;
         }
      }

      #endregion
      #region -- XML Generate values methods

      private bool GenerateElement(XmlSchemaElement e, bool root,
         InstanceGroup parentElem, XmlSchemaAny any)
      {
         XmlSchemaElement eGlobalDecl = e;

         if (!e.RefName.IsEmpty)
         {
            eGlobalDecl =
               (XmlSchemaElement)schemaSet.GlobalElements[e.QualifiedName];
         }
         if (!eGlobalDecl.IsAbstract)
         {
            InstanceElement elem =
               (InstanceElement)elementTypesProcessed[eGlobalDecl];
            InstanceElement gelem = elem;

            // only count complex types
            var cmpxType =
               eGlobalDecl.ElementSchemaType as XmlSchemaComplexType;
            if (cmpxType != null)
               m_TagInspector.Count(eGlobalDecl.Name);

            // check elements in use to prevent cycling...
            //if (m_TagInspector.PreventCycling)
            //{
               if (elem != null)
               {
                  if (!m_TagInspector.Inspect(elem.QualifiedName.Name))
                  {
                     Debug.Assert(!root);
                     if (any == null && e.MinOccurs > 0)
                     {  // if not generating for any or optional ref to
                        // cyclic global element
                        decimal occurs = e.MaxOccurs;
                        if (e.MaxOccurs >= maxThreshold)
                        {
                           occurs = maxThreshold;
                        }
                        if (e.MinOccurs > occurs)
                        {
                           occurs = e.MinOccurs;
                        }

                        // TODO: Investigate how to do a deep copy instead...
                        parentElem.AddChild(elem.Clone(occurs));
                     }
                     return false;
                  }
               }
            //}

            if (elem == null)
               elem = new InstanceElement(eGlobalDecl.QualifiedName);

            if (root)
            {
               instanceRoot = elem;
            }
            else
            {
               parentElem.AddChild(elem);
            }

            // get minOccurs, maxOccurs alone from the current particle,
            // everything else pick up from globalDecl
            if (any != null)
            {  // element from any
               elem.Occurs = any.MaxOccurs >= maxThreshold ?
                  maxThreshold : any.MaxOccurs;
               elem.Occurs = any.MinOccurs > elem.Occurs ?
                  any.MinOccurs : elem.Occurs;
            }
            else
            {
               elem.Occurs = e.MaxOccurs >= maxThreshold ?
                  maxThreshold : e.MaxOccurs;
               elem.Occurs = e.MinOccurs > elem.Occurs ?
                  e.MinOccurs : elem.Occurs;
            }
            elem.DefaultValue = eGlobalDecl.DefaultValue;
            elem.FixedValue = eGlobalDecl.FixedValue;
            elem.IsNillable = eGlobalDecl.IsNillable;

            if (eGlobalDecl.ElementSchemaType == AnyType)
            {
               elem.ValueGenerator = XmlValueGenerator.AnyGenerator;
            }
            else
            {
               XmlSchemaComplexType ct =
                  eGlobalDecl.ElementSchemaType as XmlSchemaComplexType;
               if (ct != null)
               {
                  // check 'preventCycling' above
                  if (gelem == null)
                     elementTypesProcessed.Add(eGlobalDecl, elem);
                  if (!ct.IsAbstract)
                  {
                     elem.IsMixed = ct.IsMixed;
                     ProcessComplexType(ct, elem);
                  }
                  else
                  { // Ct is abstract, need to generate instance elements with xsi:type
                     XmlSchemaComplexType dt = GetDerivedType(ct);
                     if (dt != null)
                     {
                        elem.XsiType = dt.QualifiedName;
                        ProcessComplexType(dt, elem);
                     }
                  }
               }
               else
               { //elementType is XmlSchemaSimpleType
                  elem.ValueGenerator = XmlValueGenerator.CreateGenerator(
                     eGlobalDecl.ElementSchemaType.Datatype, listLength);
               }
            }
            if (elem.ValueGenerator != null &&
               elem.ValueGenerator.Prefix == null)
            {
               elem.ValueGenerator.Prefix = elem.QualifiedName.Name;
            }
            return true;
         } // End of e.IsAbstract
         return false;
      }

      private void GenerateAttributeWildCard(XmlSchemaComplexType ct, InstanceElement elem)
      {
         char[] whitespace = new char[] { ' ', '\t', '\n', '\r' };
         InstanceAttribute attr = null;
         XmlSchemaAttribute anyAttr = null;

         XmlSchemaAnyAttribute attributeWildCard = ct.AttributeWildcard;
         XmlSchemaObjectTable attributes = ct.AttributeUses;

         string namespaceList = attributeWildCard.Namespace;
         if (namespaceList == null)
         {
            namespaceList = "##any";
         }
         if (attributeWildCard.ProcessContents == XmlSchemaContentProcessing.Skip || attributeWildCard.ProcessContents == XmlSchemaContentProcessing.Lax)
         {
            if (namespaceList == "##any" || namespaceList == "##targetNamespace")
            {
               attr = new InstanceAttribute(new XmlQualifiedName("any_Attr", rootTargetNamespace));
            }
            else if (namespaceList == "##local")
            {
               attr = new InstanceAttribute(new XmlQualifiedName("any_Attr", string.Empty));
            }
            else if (namespaceList == "##other")
            {
               attr = new InstanceAttribute(new XmlQualifiedName("any_Attr", "otherNS"));
            }
            if (attr != null)
            {
               attr.ValueGenerator = XmlValueGenerator.AnySimpleTypeGenerator;
               elem.AddAttribute(attr);
               return;
            }
         }
         switch (namespaceList)
         {
            case "##any":
            case "##targetNamespace":
               anyAttr = GetAttributeFromNS(rootTargetNamespace, attributes);
               break;

            case "##other":
               XmlSchema anySchema = GetParentSchema(attributeWildCard);
               anyAttr = GetAttributeFromNS(anySchema.TargetNamespace, true, attributes);
               break;

            case "##local": //Shd get local elements in some schema
               anyAttr = GetAttributeFromNS(string.Empty, attributes);
               break;

            default:
               foreach (string ns in attributeWildCard.Namespace.Split(whitespace))
               {
                  if (ns == "##local")
                  {
                     anyAttr = GetAttributeFromNS(string.Empty, attributes);
                  }
                  else if (ns == "##targetNamespace")
                  {
                     anyAttr = GetAttributeFromNS(rootTargetNamespace, attributes);
                  }
                  else
                  {
                     anyAttr = GetAttributeFromNS(ns, attributes);
                  }
                  if (anyAttr != null)
                  { //Found match
                     break;
                  }
               }
               break;
         }
         if (anyAttr != null)
         {
            GenerateInstanceAttribute(anyAttr, elem);
         }
         else
         { //Write comment in generated XML that match for wild card cd not be found.
            if (elem.Comment.Length == 0)
            { //For multiple attribute wildcards in the same element, generate comment only once
               elem.Comment.Append(" Attribute Wild card could not be matched. Generated XML may not be valid. ");
            }
         }
      }

      private void GenerateAttribute(XmlSchemaObjectTable attributes, InstanceElement elem)
      {
         IDictionaryEnumerator ienum = attributes.GetEnumerator();
         while (ienum.MoveNext())
         {
            if (ienum.Value is XmlSchemaAttribute)
            {
               GenerateInstanceAttribute((XmlSchemaAttribute)ienum.Value, elem);
            }
         }
      }

      private void GenerateInstanceAttribute(XmlSchemaAttribute attr, InstanceElement elem)
      {
         if (attr.Use == XmlSchemaUse.Prohibited || attr.AttributeSchemaType == null)
         {
            return;
         }
         InstanceAttribute iAttr = new InstanceAttribute(attr.QualifiedName);
         iAttr.DefaultValue = attr.DefaultValue;
         iAttr.FixedValue = attr.FixedValue;
         iAttr.AttrUse = attr.Use;
         iAttr.ValueGenerator = XmlValueGenerator.CreateGenerator(attr.AttributeSchemaType.Datatype, listLength);
         if (iAttr.ValueGenerator != null && iAttr.ValueGenerator.Prefix == null)
         {
            iAttr.ValueGenerator.Prefix = iAttr.QualifiedName.Name;
         }
         elem.AddAttribute(iAttr);
      }


      private void GenerateParticle(XmlSchemaParticle particle, bool root, InstanceGroup iGrp)
      {
         decimal max;
         max = particle.MaxOccurs >= maxThreshold ? maxThreshold : particle.MaxOccurs;
         max = particle.MinOccurs > max ? particle.MinOccurs : max;

         if (particle is XmlSchemaSequence)
         {
            XmlSchemaSequence seq = (XmlSchemaSequence)particle;
            InstanceGroup grp = new InstanceGroup();
            grp.Occurs = max;
            iGrp.AddChild(grp);
            GenerateGroupBase(seq, grp);
         }
         else if (particle is XmlSchemaChoice)
         {
            XmlSchemaChoice ch = (XmlSchemaChoice)particle;

            if (ch.MaxOccurs == 1 && _Asset == null)
            {
               XmlSchemaParticle pt = (XmlSchemaParticle)(ch.Items[0]);
               GenerateParticle(pt, false, iGrp);
            }
            else
            {
               InstanceGroup grp = new InstanceGroup();
               grp.Occurs = max;
               grp.IsChoice = true;
               iGrp.AddChild(grp);
               GenerateGroupBase(ch, grp);
            }
         }
         else if (particle is XmlSchemaAll)
         {
            GenerateAll((XmlSchemaAll)particle, iGrp);
         }
         else if (particle is XmlSchemaElement)
         {
            XmlSchemaElement elem = particle as XmlSchemaElement;
            XmlSchemaChoice ch = null;
            if (!elem.RefName.IsEmpty)
            {
               ch = GetSubstitutionChoice(elem);
            }
            if (ch != null)
            {
               GenerateParticle(ch, false, iGrp);
            }
            else
            {
               GenerateElement(elem, false, iGrp, null);
            }
         }
         else if (particle is XmlSchemaAny && particle.MinOccurs > 0)
         { //Generate any only if we should
            GenerateAny((XmlSchemaAny)particle, iGrp);
         }

      }

      private void GenerateGroupBase(XmlSchemaGroupBase gBase, InstanceGroup grp)
      {
         foreach (XmlSchemaParticle particle1 in gBase.Items)
         {
            GenerateParticle(particle1, false, grp);
         }
      }

      private void GenerateAll(XmlSchemaAll all, InstanceGroup grp)
      {
         XmlSchemaParticle pt;
         for (int i = all.Items.Count; i > 0; i--)
         {
            pt = (XmlSchemaParticle)(all.Items[i - 1]);
            GenerateParticle(pt, false, grp);
         }

      }

      private void GenerateAny(XmlSchemaAny any, InstanceGroup grp)
      {
         InstanceElement parentElem = grp as InstanceElement;
         char[] whitespace = new char[] { ' ', '\t', '\n', '\r' };
         InstanceElement elem = null;
         XmlSchemaElement anyElem = null;
         string namespaceList = any.Namespace;
         if (namespaceList == null)
         { //no namespace defaults to "##any"
            namespaceList = "##any";
         }
         if (any.ProcessContents == XmlSchemaContentProcessing.Skip || any.ProcessContents == XmlSchemaContentProcessing.Lax)
         {
            if (namespaceList == "##any" || namespaceList == "##targetNamespace")
            {
               elem = new InstanceElement(new XmlQualifiedName("any_element", rootTargetNamespace));
            }
            else if (namespaceList == "##local")
            {
               elem = new InstanceElement(new XmlQualifiedName("any_element", string.Empty));
            }
            else if (namespaceList == "##other")
            {
               elem = new InstanceElement(new XmlQualifiedName("any_element", "otherNS"));
            }
            if (elem != null)
            {
               elem.ValueGenerator = XmlValueGenerator.AnyGenerator;
               elem.Occurs = any.MaxOccurs >= maxThreshold ? maxThreshold : any.MaxOccurs;
               elem.Occurs = any.MinOccurs > elem.Occurs ? any.MinOccurs : elem.Occurs;
               grp.AddChild(elem);
               return;
            }
         }
         //ProcessContents = strict || namespaceList is actually a list of namespaces
         switch (namespaceList)
         {
            case "##any":
            case "##targetNamespace":
               anyElem = GetElementFromNS(rootTargetNamespace);
               break;

            case "##other":
               XmlSchema anySchema = GetParentSchema(any);
               anyElem = GetElementFromNS(anySchema.TargetNamespace, true);
               break;

            case "##local": //Shd get local elements in some schema
               anyElem = GetElementFromNS(string.Empty);
               break;

            default:
               foreach (string ns in namespaceList.Split(whitespace))
               {
                  if (ns == "##targetNamespace")
                  {
                     anyElem = GetElementFromNS(rootTargetNamespace);
                  }
                  else if (ns == "##local")
                  {
                     anyElem = GetElementFromNS(string.Empty);
                  }
                  else
                  {
                     anyElem = GetElementFromNS(ns);
                  }
                  if (anyElem != null)
                  { //found a match
                     break;
                  }
               }
               break;
         }
         if (anyElem != null && GenerateElement(anyElem, false, grp, any))
         {
            return;
         }
         else
         { //Write comment in generated XML that match for wild card cd not be found.
            if (parentElem == null)
            {
               parentElem = GetParentInstanceElement(grp);
            }
            if (parentElem.Comment.Length == 0)
            { //For multiple wildcards in the same element, generate comment only once
               parentElem.Comment.Append(" Element Wild card could not be matched. Generated XML may not be valid. ");
            }
         }
      }

      #endregion
      #region -- XML Substitution Groups support

      private void BuildSubstitutionGroups()
      {
         foreach (XmlSchemaElement element in schemaSet.GlobalElements.Values)
         {
            XmlQualifiedName head = element.SubstitutionGroup;
            if (!head.IsEmpty)
            {
               if (substitutionGroupsTable == null)
               {
                  substitutionGroupsTable = new Hashtable();
               }
               SubstitutionGroupWrapper substitutionGroup = (SubstitutionGroupWrapper)substitutionGroupsTable[head];
               if (substitutionGroup == null)
               {
                  substitutionGroup = new SubstitutionGroupWrapper();
                  substitutionGroup.Head = head;
                  substitutionGroupsTable.Add(head, substitutionGroup);
               }
               ArrayList members = substitutionGroup.Members;
               if (!members.Contains(element))
               { //Members might contain element if the same schema is included and imported through different paths. Imp, hence will be added to set directly
                  members.Add(element);
               }
            }
         }
         if (substitutionGroupsTable != null)
         { //There were subst grps in the schema
            foreach (SubstitutionGroupWrapper substGroup in substitutionGroupsTable.Values)
            {
               ResolveSubstitutionGroup(substGroup);
            }
         }
      }

      private void ResolveSubstitutionGroup(SubstitutionGroupWrapper substitutionGroup)
      {
         ArrayList newMembers = null;
         XmlSchemaElement headElement = (XmlSchemaElement)schemaSet.GlobalElements[substitutionGroup.Head];
         if (substitutionGroup.Members.Contains(headElement))
         {// already checked
            return;
         }
         foreach (XmlSchemaElement element in substitutionGroup.Members)
         {
            //Chain to other head's that are members of this head's substGroup
            SubstitutionGroupWrapper g = (SubstitutionGroupWrapper)substitutionGroupsTable[element.QualifiedName];
            if (g != null)
            {
               ResolveSubstitutionGroup(g);
               foreach (XmlSchemaElement element1 in g.Members)
               {
                  if (element1 != element)
                  { //Exclude the head
                     if (newMembers == null)
                     {
                        newMembers = new ArrayList();
                     }
                     newMembers.Add(element1);
                  }
               }
            }
         }
         if (newMembers != null)
         {
            foreach (XmlSchemaElement newMember in newMembers)
            {
               substitutionGroup.Members.Add(newMember);
            }
         }
         substitutionGroup.Members.Add(headElement);
      }

      private XmlSchemaChoice GetSubstitutionChoice(XmlSchemaElement element)
      {
         if (substitutionGroupsTable == null)
         {
            BuildSubstitutionGroups();
         }
         if (substitutionGroupsTable != null)
         {
            SubstitutionGroupWrapper substitutionGroup = (SubstitutionGroupWrapper)substitutionGroupsTable[element.QualifiedName];
            if (substitutionGroup != null)
            { //Element is head of a substitutionGroup
               XmlSchemaChoice choice = new XmlSchemaChoice();
               foreach (XmlSchemaElement elem in substitutionGroup.Members)
               {
                  choice.Items.Add(elem);
               }
               XmlSchemaElement headElement = (XmlSchemaElement)schemaSet.GlobalElements[element.QualifiedName];
               if (headElement.IsAbstract)
               { //Should not generate the abstract element
                  choice.Items.Remove(headElement);
               }
               choice.MinOccurs = element.MinOccurs;
               choice.MaxOccurs = element.MaxOccurs;
               return choice;
            }
         }
         return null;
      }

      #endregion
      #region -- XML Writer based processing

      private void ProcessInstanceTree(InstanceElement rootElement)
      {
         if (rootElement != null)
         {
            instanceElementsProcessed.Add(rootElement, rootElement);
            writer.WriteStartElement(rootElement.QualifiedName.Name, rootTargetNamespace);
            writer.WriteAttributeString("xmlns", "xsi", null, NsXsi);
            ProcessElementAttrs(rootElement);
            ProcessComment(rootElement);
            CheckIfMixed(rootElement);
            if (rootElement.ValueGenerator != null)
            {
               if (rootElement.IsFixed)
               {
                  writer.WriteString(rootElement.FixedValue);
               }
               else if (rootElement.HasDefault)
               {
                  writer.WriteString(rootElement.DefaultValue);
               }
               else
               {
                  writer.WriteString(rootElement.ValueGenerator.GenerateValue());
               }
            }
            else
            {
               InstanceGroup group = rootElement.Child;
               while (group != null)
               {
                  ProcessGroup(group);
                  group = group.Sibling;
               }
            }
            writer.WriteEndElement();
         }
         else
         {
            writer.WriteComment("Schema did not lead to generation of a valid XML document");
         }
      }

      private void ProcessElement(InstanceElement elem)
      {
         if (instanceElementsProcessed[elem] != null)
         {
            return;
         }
         instanceElementsProcessed.Add(elem, elem);
         for (int i = 0; i < elem.Occurs; i++)
         {
            writer.WriteStartElement(elem.QualifiedName.Name, elem.QualifiedName.Namespace);
            ProcessElementAttrs(elem);
            ProcessComment(elem);
            CheckIfMixed(elem);
            if (elem.IsNillable)
            {
               if (elem.GenNil)
               {
                  // TODO: manage nillable...
                  //WriteNillable();
                  elem.GenNil = false;
                  writer.WriteEndElement();
                  continue;
               }
               else
               {
                  elem.GenNil = true;
               }
            }

            if (elem.ValueGenerator != null)
            {
               if (elem.IsFixed)
               {
                  writer.WriteString(elem.FixedValue);
               }
               else if (elem.HasDefault)
               {
                  writer.WriteString(elem.DefaultValue);
               }
               else
               {
                  writer.WriteString(elem.ValueGenerator.GenerateValue());
               }
            }
            else
            {
               InstanceGroup childGroup = elem.Child;
               while (childGroup != null)
               {
                  ProcessGroup(childGroup);
                  childGroup = childGroup.Sibling;
               }
            }
            writer.WriteEndElement();
         }
         instanceElementsProcessed.Remove(elem);
      }

      private void ProcessComment(InstanceElement elem)
      {
         if (elem.Comment.Length > 0)
         {
            writer.WriteComment(elem.Comment.ToString());
         }
      }

      private void CheckIfMixed(InstanceElement mixedElem)
      {
         if (mixedElem.IsMixed)
         {
            writer.WriteString("text");
         }
      }

      //private void WriteNillable()
      //{
      //   writer.WriteStartAttribute(XsdNil.Name, XsdNil.Namespace);
      //   writer.WriteString("true");
      //   writer.WriteEndAttribute();
      //}

      private void ProcessElementAttrs(InstanceElement elem)
      {
         if (elem.XsiType != XmlQualifiedName.Empty)
         {
            if (elem.XsiType.Namespace != string.Empty)
            {
               writer.WriteStartAttribute("xsi", "type", null);
               writer.WriteQualifiedName(elem.XsiType.Name, elem.XsiType.Namespace);
               writer.WriteEndAttribute();
            }
            else
            {
               writer.WriteAttributeString("xsi", "type", null, elem.XsiType.Name);
            }
         }

         InstanceAttribute attr = elem.FirstAttribute;
         while (attr != null)
         {
            if (attr.AttrUse != XmlSchemaUse.Prohibited)
            {
               if (attr.QualifiedName.Namespace == NsXml)
               {
                  writer.WriteStartAttribute("xml", attr.QualifiedName.Name, attr.QualifiedName.Namespace);
               }
               else
               {
                  writer.WriteStartAttribute(attr.QualifiedName.Name, attr.QualifiedName.Namespace);
               }
               if (attr.HasDefault)
               {
                  writer.WriteString(attr.DefaultValue);
               }
               else if (attr.IsFixed)
               {
                  writer.WriteString(attr.FixedValue);
               }
               else
               {
                  writer.WriteString(attr.ValueGenerator.GenerateValue());
               }
               writer.WriteEndAttribute();
            }
            attr = attr.NextAttribute;
         }
      }

      #endregion
      #region -- XML Asset based processing

      public string GetEntityName(InstanceElement element)
      {
         String name = String.Empty;
         var parent = element.Parent;
         while (parent != null)
         {
            if (parent.QualifiedName == null)
            {
               parent = parent.Parent;
               continue;
            }
            name = parent.QualifiedName.Name;
            break;
         }
         return name;
      }

      private QualifiedNameInfo GetEntityQualifiedName(InstanceElement element)
      {
         string ename = GetEntityName(element);
         if (String.IsNullOrWhiteSpace(ename))
            ename = null;
         return new QualifiedNameInfo(ename);
      }

      private void AssetProcessInstanceTree(InstanceElement rootElement)
      {
         if (rootElement != null)
         {
            instanceElementsProcessed.Add(rootElement, rootElement);
            var asset = _Asset.AddElement(rootElement);
            asset.EntityQualifiedName = GetEntityQualifiedName(rootElement);
            asset.ElementType = ElementType.root;

            AssetProcessElementAttrs(rootElement, asset);
            AssetProcessComment(rootElement, asset);
            asset.IsMixed = rootElement.IsMixed;
            if (rootElement.ValueGenerator != null)
            {
               if (rootElement.IsFixed)
               {
                  asset.FixedValue = (rootElement.FixedValue);
               }
               else if (rootElement.HasDefault)
               {
                  asset.DefaultValue = (rootElement.DefaultValue);
               }
               else
               {
                  asset.SampleValue = (rootElement.ValueGenerator.GenerateValue());
               }
            }
            else
            {
               InstanceGroup group = rootElement.Child;
               while (group != null)
               {
                  ProcessGroup(group);
                  group = group.Sibling;
               }
            }
            asset.GetFullPath();
         }
         else
         {
            throw new Exception(
               "Schema did not lead to generation of a valid XML document");
         }
      }

      private void AssetProcessElement(InstanceElement elem)
      {
         if (instanceElementsProcessed[elem] != null)
         {
            return;
         }
         instanceElementsProcessed.Add(elem, elem);
         for (int i = 0; i < elem.Occurs; i++)
         {
            var asset = _Asset.AddElement(elem);
            asset.EntityQualifiedName = GetEntityQualifiedName(elem);

            AssetProcessElementAttrs(elem, asset);
            AssetProcessComment(elem, asset);
            asset.IsMixed = elem.IsMixed;
            asset.IsNillable = elem.IsNillable;

            if (elem.ValueGenerator != null)
            {
               if (elem.IsFixed)
               {
                  asset.FixedValue = (elem.FixedValue);
               }
               else if (elem.HasDefault)
               {
                  asset.DefaultValue = (elem.DefaultValue);
               }
               else
               {
                  asset.SampleValue = (elem.ValueGenerator.GenerateValue());
               }
            }
            else
            {
               asset.ElementType = ElementType.type;
               InstanceGroup childGroup = elem.Child;
               while (childGroup != null)
               {
                  ProcessGroup(childGroup);
                  childGroup = childGroup.Sibling;
               }
            }
            asset.GetFullPath();
         }
         instanceElementsProcessed.Remove(elem);
      }

      private void AssetProcessElementAttrs(InstanceElement elem, XmlElementInfo asset)
      {
         //if (elem.XsiType != XmlQualifiedName.Empty)
         //{
         //   if (elem.XsiType.Namespace != string.Empty)
         //   {
         //      writer.WriteStartAttribute("xsi", "type", null);
         //      writer.WriteQualifiedName(elem.XsiType.Name, elem.XsiType.Namespace);
         //      writer.WriteEndAttribute();
         //   }
         //   else
         //   {
         //      writer.WriteAttributeString("xsi", "type", null, elem.XsiType.Name);
         //   }
         //}

         string fpath = asset.GetFullPath();
         InstanceAttribute attr = elem.FirstAttribute;
         while (attr != null)
         {
            var assetAttr = _Asset.AddAttribute(asset, attr);
            if (attr.AttrUse != XmlSchemaUse.Prohibited)
            {

               if (attr.HasDefault)
               {
                  assetAttr.DefaultValue = (attr.DefaultValue);
               }
               else if (attr.IsFixed)
               {
                  assetAttr.FixedValue = (attr.FixedValue);
               }
               else
               {
                  assetAttr.SampleValue = (attr.ValueGenerator.GenerateValue());
               }
            }

            assetAttr.EntityQualifiedName =
               new QualifiedNameInfo(asset.ElementQualifiedName.Name);
            assetAttr.SetFullPath(
               fpath == null ? "" : fpath + "@" +
                  assetAttr.ElementQualifiedName.Name);
            asset.SetAttributes();

            attr = attr.NextAttribute;
         }
      }

      private AssetGroupItem FindElement(
         XmlSchemaElement element, XmlQualifiedName name)
      {
         XmlSchemaComplexType complexType =
            element.ElementSchemaType as XmlSchemaComplexType;
         if (complexType == null || complexType.ContentTypeParticle == null)
            return null;
         XmlSchemaSequence sequence =
            complexType.ContentTypeParticle as XmlSchemaSequence;
         if (sequence == null)
            return null;

         AssetGroupItem item = null;
         foreach (var p in sequence.Items)
         {
            if (p is XmlSchemaElement)
            {
               var elmt = p as XmlSchemaElement;
               if (elmt.QualifiedName.Equals(name))
               {
                  item = new AssetGroupItem(
                     ElementType.element, ElementGroup.NotApplicable, elmt);
                  break;
               }
               item = FindElement(elmt, name);
               if (item != null)
                  break;
            }
         }
         return item;
      }

      private AssetGroupItem TraverseFindParent(
         InstanceGroup parent, XmlQualifiedName name)
      {
         if (parent.QualifiedName == null)
            return TraverseFindParent(parent.Parent, name);
         XmlSchemaElement schemaElement =
              (XmlSchemaElement)schemaSet.GlobalElements[parent.QualifiedName];
         if (schemaElement == null)
            return TraverseFindParent(parent.Parent, name);

         AssetGroupItem item = FindElement(schemaElement, name);
         return item;
      }

      private AssetGroupItem AssetGetSequenceInstance(InstanceElement element)
      {
         AssetGroupItem item = null;

         var parent = element.Parent;
         while (parent != null)
         {
            if (parent.QualifiedName == null)
            {
               parent = parent.Parent;
               continue;
            }
            break;
         }
         if (parent == null)
            return item;

         XmlSchemaElement schemaElement =
            (XmlSchemaElement)schemaSet.GlobalElements[parent.QualifiedName];
         if (schemaElement == null)
         {
            return TraverseFindParent(parent.Parent, element.QualifiedName);
         }

         XmlSchemaComplexType type = (XmlSchemaComplexType)
            schemaSet.GlobalTypes[schemaElement.SchemaTypeName];
         if (type == null)
            return null;

         XmlSchemaObjectCollection col;
         XmlSchemaSequence sequence = 
            type.ContentTypeParticle as XmlSchemaSequence;
         if (sequence == null)
         {
            XmlSchemaChoice cho = type.ContentTypeParticle as XmlSchemaChoice;
            if (cho == null)
               return item;
            col = cho.Items;
         }
         else
         {
            col = sequence.Items;
         }

         foreach(XmlSchemaObject i in col) // sequence.Items)
         {
            if (i is XmlSchemaChoice)
            {
               var cho = i as XmlSchemaChoice;
               foreach(XmlSchemaElement c in cho.Items)
               {         
                  if (c.QualifiedName.Equals(element.QualifiedName))
                  {
                     item = new AssetGroupItem(
                        ElementType.type, ElementGroup.OptionOne, c);
                     break;
                  }
               }
            }
            else if (i is XmlSchemaElement)
            {
               var ele = i as XmlSchemaElement;
               if (ele.QualifiedName.Equals(element.QualifiedName))
               {
                  item = new AssetGroupItem(
                     ElementType.element, ElementGroup.NotApplicable, ele);
                  break;
               }
            }
         }
         return item;
      }

      private void SetOccurs(AssetGroupItem group, IAssetElement asset)
      {
         if (group == null)
            return;
         asset.ElementType = group.ElementType;
         if (group.Item is XmlSchemaElement el)
         {
            String mx = (el.MaxOccursString ?? DEFAULT_OCCUR);
            asset.Occurs = "(" +
               (el.MinOccursString ?? DEFAULT_OCCUR) + " : " +
               (mx.ToLower() == "unbounded" ? "*" : mx) + ")";
            return;
         }
         //if (group.Item is XmlSchemaChoice ch)
         //{
         //   asset.Occurs = String.Empty;
         //}
      }

      private void AssetAnnotation(InstanceElement element, IAssetElement asset)
      {
         //XmlSchemaElement schemaElement =
         //   (XmlSchemaElement)schemaSet.GlobalElements[element.QualifiedName];

         // find sequence/choice group item and set occurances...
         AssetGroupItem sequenceElement = AssetGetSequenceInstance(element);
         if (sequenceElement == null)
            return;

         SetOccurs(sequenceElement, asset);
         if (sequenceElement == null)
            return;

         // a schema-element was found in the Grobal list?
         if (!(sequenceElement.Item is XmlSchemaElement schemaElement))
         {
            return;
         }

         if (schemaElement.ElementSchemaType != null)
         {
            var ct = schemaElement.ElementSchemaType as XmlSchemaComplexType;

            string sname;
            if (ct == null)
               sname = ((XmlSchemaSimpleType)schemaElement.ElementSchemaType).
                  Datatype.ValueType.Name;
            else
               sname = ((XmlSchemaComplexType)
                  schemaElement.ElementSchemaType).Name;
            if (sname != null)
               asset.QualifiedTypeNames.Add(new QualifiedNameInfo(sname));
         }

         if (schemaElement.Annotation == null)
         {
            GetGlobalElementAnnotation(schemaElement, asset);
            return;
         }
         XmlSchemaInspector.AssetSchemaAnnotation(
            schemaElement.Annotation, asset);
      }

      private void GetGlobalElementAnnotation(
         XmlSchemaElement element, IAssetElement asset)
      {
         foreach(XmlSchemaElement i in schemaSet.GlobalElements.Values)
         {
            if (i.Name == element.QualifiedName.Name &&
               i.QualifiedName.Namespace == element.QualifiedName.Namespace)
            {
               if (i.Annotation == null)
                  break;
               XmlSchemaInspector.AssetSchemaAnnotation(i.Annotation, asset);
            }
         }
      }

      private void AssetProcessComment(InstanceElement elem, IAssetElement asset)
      {
         AssetAnnotation(elem, asset);
         if (elem.Comment.Length > 0)
         {
            asset.CommentText = elem.Comment.ToString();
         }
      }

      #endregion

   }
}

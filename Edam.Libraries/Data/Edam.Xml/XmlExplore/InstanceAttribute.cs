// based on original from Microsoft:
//    https://github.com/mganss/XmlSchemaClassGenerator

namespace Edam.Xml.XmlExplore {
    using System.Xml;
    using System.Xml.Schema;

    public class InstanceAttribute : InstanceObject{
        XmlSchemaUse attrUse;
        InstanceAttribute nextAttribute;

        internal InstanceAttribute(XmlQualifiedName name) {
            this.QualifiedName = new XmlQualifiedName(name.Name, name.Namespace);
        }
        
        internal XmlSchemaUse AttrUse {
            get {
                return attrUse;
            }
            set {
                attrUse = value;
            }
        }
        
        internal InstanceAttribute NextAttribute {
            get {
                return nextAttribute;
            }
            set {
                nextAttribute = value;
            }
        }

      }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Objects;
using Edam.DataObjects.DataCodes;

namespace Edam.DataObjects.Models
{

   public class ModelColumnInfo : BaseColumnInfo
   {

      public override ObjectValueType ValueType
      {
         get { return ElementValue.ValueType; }
         set { ElementValue.ValueType = value; }
      }

      private ElementItemInfo m_Element = null;
      public ElementItemInfo Element
      {
         get { return m_Element; }
         set
         {
            m_Element = value;
            ElementValue = value;
         }
      }

      public ObjectGridable Gridable
      {
         get
         {
            return m_Element == null ?
               ObjectGridable.Show : m_Element.Gridable;
         }
      }

      public bool IsGridable
      {
         get
         {
            return Gridable == ObjectGridable.Show ||
               Gridable == ObjectGridable.Edit;
         }
      }

      public ObservableCollection<DataCodeInfo> LookUpItems { get; set; }

      private ObjectValueBase m_ElementValue = null;
      public ObjectValueBase ElementValue
      {
         get { return m_ElementValue; }
         set
         {
            m_ElementValue = value ?? new ObjectValueBase();
         }
      }

      public ModelColumnInfo() : base()
      {
         EditControl = null;
         EditControlType = ModelColumnControlType.Text;
      }

      /// <summary>
      /// If there is a Disposable method for the EditControl call it...
      /// </summary>
      public void Dispose()
      {
         if (EditControl is IDisposable f)
         {
            f.Dispose();
         }
         EditControl = null;
      }
   }

}

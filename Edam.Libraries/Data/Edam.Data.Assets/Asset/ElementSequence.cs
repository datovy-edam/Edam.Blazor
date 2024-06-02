using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.AssetManagement;

namespace Edam.Data.Asset
{

   public class ElementSequence
   {
      private const string SEPARATOR = ".";
      public int SequenceNo;
      public int GroupNo;
      public ElementGroup Group;
      public OccuranceInfo Occurance = new OccuranceInfo();
      public string Comment;
      public ElementSequence()
      {
         ClearFields();
      }
      public ElementSequence(string value)
      {
         FromString(value);
      }
      public ElementSequence(int sequenceNo)
      {
         ClearFields();
         SequenceNo = sequenceNo;
      }
      public ElementSequence(int sequenceNo, int groupNo, ElementGroup group)
      {
         SequenceNo = sequenceNo;
         GroupNo = groupNo;
         Group = group;
      }
      public ElementSequence(ElementSequence sequence, ElementGroup group)
      {
         SequenceNo = sequence.SequenceNo;
         GroupNo = sequence.GroupNo;
         Group = group;
      }
      private void ClearFields()
      {
         SequenceNo = 0;
         GroupNo = 0;
         Group = ElementGroup.Sequence;
      }
      public new string ToString()
      {
         return SequenceNo.ToString() + SEPARATOR + GroupNo.ToString() +
            SEPARATOR + Group.ToString() + (Group == ElementGroup.Choise ?
               SEPARATOR + Occurance.Text : String.Empty);
      }
      public bool FromString(string value)
      {
         string[] parts = value.Split(SEPARATOR);
         if (parts.Length >= 3)
         {
            SequenceNo = int.Parse(parts[0]);
            GroupNo = int.Parse(parts[1]);
            if (Enum.TryParse(typeof(ElementGroup), parts[2], out Object g))
            {
               Group = (ElementGroup)g;
            }
            if (parts.Length == 4)
            {
               Occurance = new OccuranceInfo(parts[3]);
            }
            return true;
         }
         else
         {
            ClearFields();
         }
         return false;
      }
   }

}

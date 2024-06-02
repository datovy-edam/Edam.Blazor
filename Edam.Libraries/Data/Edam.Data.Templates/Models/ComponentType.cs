using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{

   public enum GridTier
   {
      Unknown = 0,
      Small = 1,
      Medium = 2,
      Large = 3
   }

   public class GridTierInfo 
   {
      public String Label { get; set; }
      public GridTier Tier { get; set; }
      public String SizeLabel { get; set; }
      public String Format { get; set; }
      public GridTierInfo(String gridTier, String numberOfColumns = "1")
      {
         Set(gridTier, numberOfColumns);
      }

      public GridTierInfo Set(String gridTier, String numberOfColumns)
      {
         String gt = gridTier.ToLower();
         if (gt == ComponentHelper.GRID_SIZE_SMALL_LABEL)
         {
            Label = ComponentHelper.GRID_SIZE_SMALL_LABEL;
            Tier = GridTier.Small;
            SizeLabel = ComponentHelper.GRID_SIZE_SMALL;
         }
         else if (gt == ComponentHelper.GRID_SIZE_MEDIUM_LABEL)
         {
            Label = ComponentHelper.GRID_SIZE_MEDIUM_LABEL;
            Tier = GridTier.Medium;
            SizeLabel = ComponentHelper.GRID_SIZE_MEDIUM;
         }
         else if (gt == ComponentHelper.GRID_SIZE_LARGE_LABEL)
         {
            Label = ComponentHelper.GRID_SIZE_LARGE_LABEL;
            Tier = GridTier.Large;
            SizeLabel = ComponentHelper.GRID_SIZE_LARGE;
         }
         else
         {
            Label = ComponentHelper.GRID_SIZE_SMALL_LABEL;
            Tier = GridTier.Small;
            SizeLabel = ComponentHelper.GRID_SIZE_SMALL;
         }
         Format = String.Format(ComponentHelper.GRID_CLASS_FORMAT,
            SizeLabel, numberOfColumns);
         return this;
      }

      public GridTierInfo Set(String numberOfColumns)
      {
         return Set(Label, numberOfColumns);
      }
   }

   public enum ComponentType
   {
      Unknown = 0,
      Html = 1,
      XAML = 2,
      TypeScript = 3,
      StyleSheet = 4
   }

}

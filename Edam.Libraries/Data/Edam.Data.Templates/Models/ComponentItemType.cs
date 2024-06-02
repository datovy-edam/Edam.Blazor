using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{

   public enum ComponentItemType
   {
      Unknown = 0,
      ControlGridSize = 1,

      FormBody = 20,
      FormName = 21,
      FormGroupName = 22,
      FormStatusMessage = 23,

      FormControlName = 30,
      ControlName = 31,
      ControlValue = 32,
      ControlValidators = 33,

      FieldName = 50,
      FieldLabel = 51,
      FieldRequiredText = 52,
      FieldToolTip = 53,
      FieldValuePlaceholder = 54,
      FieldCodes = 56,

      ListDefinitions = 65,
      ListInit = 66,

      ItemCode = 70,
      ItemValue = 71,

      InputType = 80,
      InputId = 81,

      RowBody = 90,

      SelectorName = 91,
      ComponentName = 92,
      ComponentUpName = 93,
      FormControls = 94,

      ButtonSubmitLabel = 100,
      ButtonCancelLabel = 101
   }

}

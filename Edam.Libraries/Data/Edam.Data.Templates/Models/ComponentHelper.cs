using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using System.Text;

namespace Edam.DataObjects.Models
{

   public class ComponentHelper
   {
      public static readonly String CONTROL_GRID_SIXE = "control.gridSize";
      public static readonly String FORM_NAME = "form.name";
      public static readonly String FORM_GROUP_NAME = "form.groupName";
      public static readonly String FORM_BODY = "form.body";
      public static readonly String FORM_STATUS_MESSAGE = "form.statusMessage";
      public static readonly String FORM_CONTROL_NAME = "form.controlName";

      public static readonly String CONTROL_NAME = "control.name";
      public static readonly String CONTROL_VALUE = "control.value";
      public static readonly String CONTROL_VALIDATORS = "control.validators";

      public static readonly String FIELD_NAME = "field.name";
      public static readonly String FIELD_LABEL = "field.label";
      public static readonly String FIELD_CODES = "field.codes";
      public static readonly String FIELD_REQUIRED_TEXT = "field.requiredLabel";
      public static readonly String FIELD_TOOL_TIP = "field.toolTip";
      public static readonly String FIELD_VALUE_PLACEHOLDER = "field.placeholder";

      public static readonly String ITEM_CODE = "item.code";
      public static readonly String ITEM_VALUE = "item.value";

      public static readonly String ROW_BODY = "row.body";

      public static readonly String INPUT_TYPE = "input.type";
      public static readonly String INPUT_ID = "input.id";

      public static readonly String GRID_SIZE_DEFAULT = "12";
      public static readonly String GRID_SIZE_SMALL_LABEL = "small";
      public static readonly String GRID_SIZE_SMALL = "sm";
      public static readonly String GRID_SIZE_MEDIUM_LABEL = "medium";
      public static readonly String GRID_SIZE_MEDIUM = "md";
      public static readonly String GRID_SIZE_LARGE_LABEL = "large";
      public static readonly String GRID_SIZE_LARGE = "lg";
      public static readonly String GRID_CLASS_FORMAT = "col-{0}-{1}";

      public static readonly String ITEM_DIV_INPUT = "{0}.div.input.";
      public static readonly String ITEM_DIV_SELECT = "{0}.div.select.";
      public static readonly String ITEM_FORM = "{0}.form.";
      public static readonly String ITEM_ROW = "{0}.row.";
      public static readonly String ITEM_COMPONENT = "{0}.component.";
      public static readonly String ITEM_DECLARATIONS = "{0}.declarations.";
      public static readonly String ITEM_FORM_CONTROL = "{0}.formcontrol.";

      public static readonly String ITEM_CODE_LIST_DECLARATION = "{0}.codeHelper.define.";
      public static readonly String ITEM_CODE_LIST_INITIALIZATION = "{0}.codeHelper.init.";

      public static readonly String BUTTON_SUBMIT_LABEL = "button.submitLabel";
      public static readonly String BUTTON_CANCEL_LABEL = "button.cancelLabel";

      public static readonly String TYPE_HTML = "html";
      public static readonly String TYPE_XAML = "xaml";
      public static readonly String TYPE_TYPESCRIPT = "ts";
      public static readonly String TYPE_STYLESHEET = "css";

      public static readonly String LIST_DEFINITIONS = "list.definitions";
      public static readonly String LIST_INIT = "list.init";

      public static readonly String TRANSLATE_FORMAT = "'{0}' | translate";

      public static readonly String SELECTOR_NAME = "selector.name";
      public static readonly String COMPONENT_NAME = "component.name";
      public static readonly String COMPONENT_UP_NAME = "component.Name";
      public static readonly String FORM_CONTROLS = "form.controls";

      public static readonly String DEFAULT = "default";
      public static readonly String COMPONENT_FILE_NAME = "{0}.component.{1}";

      public static readonly String CODE_LIST = "CodeList";
      public static readonly String CODE_ID = "CodeId";
      public static readonly String CODE_VALUE = "Value";

      /// <summary>
      /// Given an element return the related element details...
      /// </summary>
      /// <param name="element">element details</param>
      /// <returns>the name of the code list is returned</returns>
      public static String GetCodeListNameIdentifier(ElementItemInfo element)
      {
         return element.Name + CODE_LIST;
      }

      /// <summary>
      /// Get CodeId Identifier string (for CodeValueHelper).
      /// </summary>
      /// <returns>the CodeId identifier string is returned</returns>
      public static String GetCodeIdIdentifier()
      {
         return CODE_ID;
      }

      /// <summary>
      /// Get Code Value identifier (for CodeValueHelper).
      /// </summary>
      /// <returns>the value identifier is returned.</returns>
      public static String GetCodeValueIdentifier()
      {
         return CODE_VALUE;
      }

      /// <summary>
      /// Given a grid size label, return its corresponding size;
      /// </summary>
      /// <param name="sizeLabel"></param>
      /// <returns></returns>
      public static String GetGridSize(String? sizeLabel = null)
      {
         sizeLabel = sizeLabel ?? GRID_SIZE_SMALL_LABEL;
         if (sizeLabel == GRID_SIZE_SMALL_LABEL)
            return GRID_SIZE_SMALL;
         return GRID_SIZE_SMALL;
      }

      public static String GetInputType(ElementItemInfo element)
      {
         String itype;
         switch(element.ValueType)
         {
            case Objects.ObjectValueType.Date:
               itype = "date";
               break;
            case Objects.ObjectValueType.DateTime:
               itype = "datetime";
               break;
            case Objects.ObjectValueType.String:
            default:
               itype = "text";
               break;
         }
         return itype;
      }

      /// <summary>
      /// Get Translate Label for given key.
      /// </summary>
      /// <param name="key"></param>
      /// <returns></returns>
      public static String GetTranslateLabel(String key)
      {
         return "{{" + String.Format(TRANSLATE_FORMAT, key) + "}}";
      }

      /// <summary>
      /// Given a type, part and name return the part name.
      /// </summary>
      /// <param name="type"></param>
      /// <param name="part"></param>
      /// <param name="name"></param>
      /// <returns></returns>
      public static String GetComponentPartName(
         ComponentType type, ComponentPartType part, String name)
      {
         string f = null;
         switch(part)
         {
            case ComponentPartType.DivisionInput:
               f = ITEM_DIV_INPUT;
               break;
            case ComponentPartType.DivisionSelect:
               f = ITEM_DIV_SELECT;
               break;
            case ComponentPartType.Form:
               f = ITEM_FORM;
               break;
            case ComponentPartType.Row:
               f = ITEM_ROW;
               break;
            case ComponentPartType.CodeDeclaration:
               f = ITEM_DECLARATIONS;
               break;
            case ComponentPartType.CodeFormControl:
               f = ITEM_FORM_CONTROL;
               break;
            case ComponentPartType.CodeComponent:
               f = ITEM_COMPONENT;
               break;
            case ComponentPartType.CodeListDeclaration:
               f = ITEM_CODE_LIST_DECLARATION;
               break;
            case ComponentPartType.CodeListInitialization:
               f = ITEM_CODE_LIST_INITIALIZATION;
               break;
            default:
               f = ITEM_DIV_INPUT;
               break;
         }
         string t = null;
         switch(type)
         {
            case ComponentType.Html:
               t = TYPE_HTML;
               break;
            case ComponentType.XAML:
               t = TYPE_XAML;
               break;
            case ComponentType.TypeScript:
               t = TYPE_TYPESCRIPT;
               break;
            case ComponentType.StyleSheet:
               t = TYPE_STYLESHEET;
               break;
            default:
               t = TYPE_HTML;
               break;
         }
         return String.Format(f, name) + t;
      }

      /// <summary>
      /// Given a text (i.e. "SomeLike" output equivalent underscord name as
      /// (i.e. "some_like")...
      /// </summary>
      /// <param name="text">any text string</param>
      /// <returns>lower underscorde line is returned</returns>
      public static String GetUnderscoredText(String text)
      {
         StringBuilder b = new StringBuilder();
         var t = text ?? String.Empty;
         var l = t.ToCharArray();
         foreach(var c in l)
         {
            if (char.IsUpper(c))
            {
               if (b.Length > 0)
                  b.Append('_');
            }
            b.Append(char.ToLower(c));
         }
         return b.ToString();
      }

      /// <summary>
      /// Given a grid size and label return the class name.
      /// </summary>
      /// <param name="size"></param>
      /// <param name="sizeLabel"></param>
      /// <returns></returns>
      public static String GetGridColumnClass(
         String size, String? sizeLabel = null)
      {
         size = size ?? GRID_SIZE_DEFAULT;
         return String.Format(GRID_CLASS_FORMAT, GetGridSize(sizeLabel), size);   
      }

      /// <summary>
      /// Get the TAG given a field-type.
      /// </summary>
      /// <remarks>
      /// Decided to do a switch over using an array since the enum values 
      /// should not be specified in a given particular order and therefore 
      /// should not be used for such purposes.
      /// </remarks>
      /// <param name="type">Field type (see PresentationFieldType)</param>
      /// <returns>the TAG label is returned if the given type is supported else
      /// null is returned</returns>
      public static String GetFieldTag(ComponentItemType type)
      {
         String tag = "<<";
         switch(type)
         {
            case ComponentItemType.ControlGridSize:
               tag += CONTROL_GRID_SIXE;
               break;
            case ComponentItemType.FormName:
               tag += FORM_NAME;
               break;
            case ComponentItemType.FormGroupName:
               tag += FORM_GROUP_NAME;
               break;
            case ComponentItemType.FormBody:
               tag += FORM_BODY;
               break;
            case ComponentItemType.FormStatusMessage:
               tag += FORM_STATUS_MESSAGE;
               break;
            case ComponentItemType.FormControlName:
               tag += FORM_CONTROL_NAME;
               break;
            case ComponentItemType.ControlName:
               tag += CONTROL_NAME;
               break;
            case ComponentItemType.ControlValue:
               tag += CONTROL_VALUE;
               break;
            case ComponentItemType.ControlValidators:
               tag += CONTROL_VALIDATORS;
               break;
            case ComponentItemType.FieldName:
               tag += FIELD_NAME;
               break;
            case ComponentItemType.FieldLabel:
               tag += FIELD_LABEL;
               break;
            case ComponentItemType.FieldRequiredText:
               tag += FIELD_REQUIRED_TEXT;
               break;
            case ComponentItemType.FieldToolTip:
               tag += FIELD_TOOL_TIP;
               break;
            case ComponentItemType.FieldValuePlaceholder:
               tag += FIELD_VALUE_PLACEHOLDER;
               break;
            case ComponentItemType.FieldCodes:
               tag += FIELD_CODES;
               break;
            case ComponentItemType.ItemCode:
               tag += ITEM_CODE;
               break;
            case ComponentItemType.ItemValue:
               tag += ITEM_VALUE;
               break;
            case ComponentItemType.RowBody:
               tag += ROW_BODY;
               break;
            case ComponentItemType.ButtonSubmitLabel:
               tag += BUTTON_SUBMIT_LABEL;
               break;
            case ComponentItemType.ButtonCancelLabel:
               tag += BUTTON_CANCEL_LABEL;
               break;
            case ComponentItemType.InputType:
               tag += INPUT_TYPE;
               break;
            case ComponentItemType.InputId:
               tag += INPUT_ID;
               break;
            case ComponentItemType.SelectorName:
               tag += SELECTOR_NAME;
               break;
            case ComponentItemType.ComponentName:
               tag += COMPONENT_NAME;
               break;
            case ComponentItemType.ComponentUpName:
               tag += COMPONENT_UP_NAME;
               break;
            case ComponentItemType.FormControls:
               tag += FORM_CONTROLS;
               break;
            case ComponentItemType.ListDefinitions:
               tag += LIST_DEFINITIONS;
               break;
            case ComponentItemType.ListInit:
               tag += LIST_INIT;
               break;
            default:
               tag = null;
               break;
         }
         if (tag != null)
            tag += ">>";
         return tag;
      }
   }

}


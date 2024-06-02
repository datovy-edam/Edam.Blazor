
// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Medias
{
   
   /// <summary>
   /// Media content types
   /// </summary>
   public enum MediaContentType
   {
      Unknown = 0,

      application_octetStream = 16,
      image_jpeg = 9,
      video_mpeg = 10,
      text_plain = 2,
      text_richtext = 3,
      application_msword = 21,
      application_officeword = 26,
      application_pdf = 12,
      text_xml = 11,
      image_png = 6,
      application_json = 16,

      image_jpeg2000 = 200,
      image_wsq = 210,
      image_bmp = 220,
      image_vector = 230,
      image_g3fax = 240
   }

}

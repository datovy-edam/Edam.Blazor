﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Edam.Data.FileSystemModel;

[Table("FileItem")]
public class FileItemInfo
{

   [Key]
   public Guid Id { get; set; } = Guid.NewGuid();

   [ForeignKey(nameof(Container))]
   public Guid ContainerId { get; set; }

   [Required]
   public ContainerInfo Container { get; set; }

   [MaxLength(1024)]
   public string FullPath { get; set; }

   [MaxLength(128)]
   public string Name { get; set; }

   [MaxLength(1024)]
   public string Description { get; set; }

   public virtual ICollection<FileItemDataInfo> DataItems { get; set; }

   public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
   public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;
   public DateTimeOffset RecordStatusDate { get; set; } = DateTimeOffset.Now;

   [MaxLength(20)]
   public string RecordStatusCode { get; set; } = "Active";

   public override string ToString()
   {
      return JsonConvert.SerializeObject(this);
   }

}

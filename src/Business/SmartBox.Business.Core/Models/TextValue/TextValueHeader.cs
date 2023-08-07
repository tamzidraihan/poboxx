using SmartBox.Business.Core.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.TextValue
{
    public class TextValueHeaderModel: BaseModel
    {
        public TextValueHeaderModel()
        {
            Collection = new List<TextValueModel>();
        }
        public List<TextValueModel> Collection { get; set; }
    }
}

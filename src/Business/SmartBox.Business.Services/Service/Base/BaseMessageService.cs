using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SmartBox.Business.Services.Service.AppMessage;

namespace SmartBox.Business.Services.Service.Base
{
    public class BaseMessageService<T> : IBaseMessageService<T> where T : class
    {
        protected readonly IAppMessageService AppMessageService;
        protected readonly IMapper Mapper;

        public BaseMessageService(IAppMessageService appMessageService, IMapper mapper)
        {
            this.AppMessageService = appMessageService;
            this.Mapper = mapper;
        }

    }
}

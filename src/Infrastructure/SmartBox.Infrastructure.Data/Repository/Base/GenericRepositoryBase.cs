using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.ParentCompany;
using SmartBox.Infrastructure.Data.Data;

namespace SmartBox.Infrastructure.Data.Repository.Base
{
    public class GenericRepositoryBase<TEntity, T> : IGenericRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly IDatabaseHelper _databaseHelper;
        protected readonly ILogger _logger;

        public GenericRepositoryBase(IDatabaseHelper databaseHelper, ILogger<T> logger)
        {
            this._databaseHelper = databaseHelper;
            this._logger = logger;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IUtilMusic.Persistence
{
    [Serializable]
    public class Config: IDisposable
    {
        public Boolean ShowNotification { get; set; }

        public void Dispose()
        {
            ConfigPersister.Save(this);
        }

        #region Singleton
        private static Config _config;

        public static Config getInstance() 
        {
            if(_config == null)
            {
                _config = ConfigPersister.Load();
            }

            return _config;
        }
        #endregion
    }

}

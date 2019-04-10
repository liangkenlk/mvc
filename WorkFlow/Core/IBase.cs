namespace TY.Core
{
    using System;
    using System.Runtime.InteropServices;

    public interface IBase
    {
    
        T Query<T>(Enum key);
        T Query<T>(string key);
        T Query<T>(string key, T defaultValue);


    }
}


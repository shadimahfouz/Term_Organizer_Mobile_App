using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WGUTermOrg
{
    public interface IAppDatabase
    {
        SQLiteAsyncConnection GetConnection();
    }
}

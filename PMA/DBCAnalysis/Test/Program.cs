using Neusoft.Reach.CANComponent.Handle;
using Neusoft.Reach.DBCAnalysis.Handle;
using Neusoft.Reach.DBCAnalysis.Infrastructure;
using System;
using Neusoft.Reach.SaveComponent.Handler;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {            
            var dbc =new  DBCHandler(@"d:\pma.dbc");
            dbc.LoadDBC();
            var wgerg = DateTime.Now.ToString("yyyyMMddHHmmss");
            DBHandler handler = DBHandler.Instance();
            handler.DeleteAllRecord();
            handler.DeleteTable();
            Console.Read();
        }
    }
}

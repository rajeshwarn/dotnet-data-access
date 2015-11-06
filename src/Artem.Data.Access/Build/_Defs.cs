using System;
using System.Collections.Generic;
using System.Text;

namespace Artem.Data.Access.Build {

    /// <summary>
    /// 
    /// </summary>
    public enum MapMemberType {
        Field,
        Property,
        Method,
        Parameter
    }

    [Flags]
    public enum PagingFlags {
        None        = 0x0000,
        RowIndex    = 0x0001,
        PageSize    = 0x0002,
        Enabled     = RowIndex | PageSize
    }

    ///// <summary>
    ///// 
    ///// </summary>
    //public class MapFileDescriptor {
    //    public string Namespace;
    //    public string ConnectionString;
    //    public string DataPrefix;
    //    public MapFileTableDescriptor[] Descriptors;
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //public class MapFileTableDescriptor {
    //    //public string ConnectionString;
    //    //public string TableName;
    //    public string ClassName;
    //    public string SelectCommand;
    //    public bool IsPartial;
    //    //public bool AllowCollectionClass;
    //    //public string CollectionClassName;
    //    //public bool AllowGatewayClass;
    //    //public string GatewayClassName;
    //    //public MapFileMethodDescriptor[] Methods;
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //internal class MapFileMethodDescriptor {
    //    public string Name;
    //    public bool Static;
    //    public string Accessor;
    //    public string Type;
    //    public string SqlCommand;
    //    public string SqlCommandType;
    //    public string SqlParams;
    //    public string SqlFile;
    //}
}

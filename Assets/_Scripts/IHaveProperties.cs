using System.Collections.Generic;

internal interface IHaveProperties
{
    Dictionary<string, object> GetProperties();
}
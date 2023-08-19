﻿using OneOf;
using OneOf.Types;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IEmployeeService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetEmployeeById?id={id}",
            ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Task<OneOf<Employee, NotFound>> GetEmployeeById(int id);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "EnableEmployee?id={id}",
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        Task<OneOf<Success, Error<string>>> EnableEmployee(int id, int enable);
    }

	
}

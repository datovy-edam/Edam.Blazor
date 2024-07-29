using Edam.Web.Models.Application;
using Edam.Data.FileSystemModel;

namespace Edam.Web.FileSystemHelper;

public class FileSystemApiClient(HttpClient httpClient)
{
   public const string SERVICE_BASE_URI = "/filesystemservice/";
   public const string SESSION_ID_PARAM = "sessionId";
   public const string INFO = "info";
   public const string LIST = "list";
   public const string ENLIST = "enlist";
   public const string DELIST = "delist";
   public const string DESCRIPTION = "description";
   public const string SESSION = "session";
   public const string CONTAINER = "container";
   public const string CONTAINER_ID_PARAM = "containerId";

   #region -- 4.00 - Manage Session and Initialization

   /// <summary>
   /// Initialize Session and setup default or initial container ID.
   /// </summary>
   /// <returns></returns>
   public async Task<ContainerInfo?> InitializeAsync()
   {
      AppSession.CurrentContainer = await GetSessionInfo("");
      return AppSession.CurrentContainer;
   }

   /// <summary>
   /// For current Session find given container ID.
   /// </summary>
   /// <param name="containerId">(optional) requested container id</param>
   /// <param name="cancellationToken">cancellation token</param>
   /// <returns>default container for session is returned</returns>
   public async Task<ContainerInfo?> GetSessionInfo(string containerId,
      CancellationToken cancellationToken = default)
   {
      var sessionId = AppSession.SessionId;
      var requestUrl = SERVICE_BASE_URI + SESSION + "/" + INFO + "?" +
         SESSION_ID_PARAM + "=" + sessionId + " &" +
         CONTAINER_ID_PARAM + "=" + containerId;

      var container = await
         httpClient.GetFromJsonAsync<ContainerInfo>(requestUrl);

      return container;
   }

   #endregion
   #region -- 4.00 - Manage Container

   /// <summary>
   /// For current Session find given container ID.
   /// </summary>
   /// <param name="containerId"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   public async Task<ContainerInfo?> GetContainer(string containerId,
      CancellationToken cancellationToken = default)
   {
      var sessionId = AppSession.SessionId;
      var requestUrl = SERVICE_BASE_URI + CONTAINER + "/" + INFO + "?" +
         SESSION_ID_PARAM + "=" + sessionId + " &" +
         CONTAINER_ID_PARAM + "=" + containerId;

      var container = await
         httpClient.GetFromJsonAsync<ContainerInfo>(requestUrl);

      return container;
   }

   /// <summary>
   /// Get/Set container by its ID.
   /// </summary>
   /// <remarks>
   /// if given id is empty, null or not found the default container will be
   /// returned</remarks>
   /// <param name="containerId">container id</param>
   /// <returns>container info is returned</returns>
   public async Task<ContainerInfo?> GetSetContainer(string containerId)
   {
      var cid = await GetContainer(containerId);
      if (cid == null)
      {
         cid = new();
      }
      AppSession.CurrentContainer = cid;
      return AppSession.CurrentContainer;
   }

   /// <summary>
   /// Get the list of all containers
   /// </summary>
   /// <returns></returns>
   public async Task<List<ContainerInfo>> GetContainerList()
   {
      var sessionId = AppSession.SessionId;
      var requestUrl = SERVICE_BASE_URI + CONTAINER + "/" + LIST + "?" +
         SESSION_ID_PARAM + "=" + sessionId;

      var list = await
         httpClient.GetFromJsonAsync<List<ContainerInfo>>(requestUrl);

      return list;
   }

   /// <summary>
   /// Get enlisted container.
   /// </summary>
   /// <remarks>
   /// if given id is empty, null or not found the default container will be
   /// returned</remarks>
   /// <param name="containerId">container id</param>
   /// <param name="description">description</param>
   /// <returns>container info is returned</returns>
   public async Task<ContainerInfo?> EnlistContainer(
      string containerId, string description)
   {
      var sessionId = AppSession.SessionId;
      var requestUrl = SERVICE_BASE_URI + CONTAINER + "/" + ENLIST + "?" +
         SESSION_ID_PARAM + "=" + sessionId + " &" +
         CONTAINER_ID_PARAM + "=" + containerId + " &" +
         DESCRIPTION + "=" + description;

      var container = await
         httpClient.GetFromJsonAsync<ContainerInfo>(requestUrl);

      return container;
   }

   /// <summary>
   /// Get enlisted container.
   /// </summary>
   /// <remarks>
   /// if given id is empty, null or not found the default container will be
   /// returned</remarks>
   /// <param name="containerId">container id</param>
   /// <returns>container info is returned</returns>
   public async Task<ContainerInfo?> DelistContainer(
      string containerId, string description)
   {
      var sessionId = AppSession.SessionId;
      var requestUrl = SERVICE_BASE_URI + CONTAINER + "/" + DELIST + "?" +
         SESSION_ID_PARAM + "=" + sessionId + " &" +
         CONTAINER_ID_PARAM + "=" + containerId;

      var container = await
         httpClient.GetFromJsonAsync<ContainerInfo>(requestUrl);

      return container;
   }

   #endregion

}

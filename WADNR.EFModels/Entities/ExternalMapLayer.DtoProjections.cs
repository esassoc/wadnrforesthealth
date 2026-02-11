using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ExternalMapLayerProjections
{
    public static readonly Expression<Func<ExternalMapLayer, ExternalMapLayerDetail>> AsDetail = x => new ExternalMapLayerDetail
    {
        ExternalMapLayerID = x.ExternalMapLayerID,
        DisplayName = x.DisplayName,
        LayerUrl = x.LayerUrl,
        LayerDescription = x.LayerDescription,
        FeatureNameField = x.FeatureNameField,
        DisplayOnPriorityLandscape = x.DisplayOnPriorityLandscape,
        DisplayOnProjectMap = x.DisplayOnProjectMap,
        DisplayOnAllOthers = x.DisplayOnAllOthers,
        IsActive = x.IsActive,
        IsTiledMapService = x.IsTiledMapService
    };
}

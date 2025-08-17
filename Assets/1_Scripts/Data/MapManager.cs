using System;

public class MapManager : IDataManager
{
    private readonly AppData _appData;

    public MapManager(AppData appData)
    {
        _appData = appData ?? throw new ArgumentNullException(nameof(appData));
    }

    public MapData GetByEvent(EventModel model)
    {
        foreach (var map in _appData.maps)
        {
            if (map.Event.date == model.date && map.Event.time == model.time)
            {
                return map;
            }
        }
        return null;
    }

    public void Add(MapData map) => _appData.maps.Add(map);
    public void Remove(MapData map) => _appData.maps.Remove(map);
}
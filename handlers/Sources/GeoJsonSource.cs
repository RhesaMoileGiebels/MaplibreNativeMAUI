using GeoJSON.Text.Feature;

namespace MapLibreNative.Maui.Handlers.Sources;

public class GeoJsonSource : SourceView
{
    public static readonly BindableProperty FeatureCollectionProperty =
        BindableProperty.Create(nameof(FeatureCollection), typeof(FeatureCollection), typeof(GeoJsonSource), null, propertyChanged: OnPropertiesChanged);

    public FeatureCollection? FeatureCollection
    {
        get => (FeatureCollection?)GetValue(FeatureCollectionProperty);
        set => SetValue(FeatureCollectionProperty, value);
    }

    private static void OnPropertiesChanged(BindableObject bindable, object oldValue, object newValue)
        => ((GeoJsonSource)bindable).PushFeatureCollection();

    /// <summary>
    /// Pushes the current <see cref="FeatureCollection"/> into the native style. Called on every
    /// data change. This is what makes both cases work:
    /// (1) the style loaded before data was set — the StyleLoaded materialise pass bailed on
    ///     <c>FeatureCollection == null</c>, so the source must be added now that data exists;
    /// (2) live data updates (e.g. a 1 Hz GPS marker) — the controller's AddGeoJsonSource is
    ///     add-or-update, so repeated pushes replace the source data in place.
    /// Without this, data set after the materialise pass never reached the map at all: the
    /// source stayed missing and every layer referencing it silently drew nothing until a
    /// style reload re-materialised the element.
    /// </summary>
    private void PushFeatureCollection()
    {
        var parentMap = FindParentMapLibreMap(this);
        if (parentMap == null || !parentMap.IsStyleLoaded) return;
        if (string.IsNullOrEmpty(SourceName)) return;
        if (FeatureCollection == null) return;
        parentMap.AddGeoJsonSource(SourceName, FeatureCollection);
        IsAdded = true;
    }

    protected override void AddLayerToParentMap()
    {
        // Find the parent MapLibreMap
        var parentMap = FindParentMapLibreMap(this);
        if (parentMap == null) return;
        if (string.IsNullOrEmpty(SourceName)) return;
        if (FeatureCollection == null) return;
        parentMap.AddGeoJsonSource(SourceName, FeatureCollection);
    }
}
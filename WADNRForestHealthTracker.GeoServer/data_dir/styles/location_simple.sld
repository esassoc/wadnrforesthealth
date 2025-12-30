<?xml version="1.0" encoding="UTF-8"?>
<StyledLayerDescriptor version="1.0.0" 
 xsi:schemaLocation="http://www.opengis.net/sld StyledLayerDescriptor.xsd" 
 xmlns="http://www.opengis.net/sld" 
 xmlns:ogc="http://www.opengis.net/ogc" 
 xmlns:xlink="http://www.w3.org/1999/xlink" 
 xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <!-- a Named Layer is the basic building block of an SLD document -->
  <NamedLayer>
    <Name>location_simple</Name>
    <UserStyle>
    <!-- Styles can have names, titles and abstracts -->
      <Title>Blue Pin Location</Title>
      <Abstract>A style that draws a marker on a point</Abstract>
      <!-- FeatureTypeStyles describe how to render different features -->
      <!-- A FeatureTypeStyle for rendering points -->
      <FeatureTypeStyle>
		<Rule>
		   <PointSymbolizer>
			 <Graphic>
			   <ExternalGraphic>
				 <OnlineResource
				   xlink:type="simple"
				   xlink:href="blue-pin.png" />
				 <Format>image/png</Format>
			   </ExternalGraphic>
			   <Size>50</Size>
			 </Graphic>
		   </PointSymbolizer>
		 </Rule>
      </FeatureTypeStyle>
    </UserStyle>
  </NamedLayer>
</StyledLayerDescriptor>
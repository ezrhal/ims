



async function initMap(container, defLat = 8.55387619375953, defLong = 125.94693580091435, searchElem = "pac-search") {
    const { AdvancedMarkerElement } = await google.maps.importLibrary("marker");
    const {PinElement} = await google.maps.importLibrary("marker");
    const myLatlng = { lat: defLat, lng: defLong };
    const map = new google.maps.Map(document.getElementById(container), {
        zoom: 15,
        center: myLatlng,
        mapId: 'DEMO_MAP_ID'
    });

    var searchMarker = new google.maps.Marker();
    var searchLatLng
    const icon = {
        size: new google.maps.Size(71, 71),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(17, 34),
        scaledSize: new google.maps.Size(35, 35),
    };
    
    const infoWindow = new google.maps.InfoWindow({ position: searchLatLng });
    infoWindow.setContent(
        'Set Project location here, <br>or drag pin to desired location'
    );

    const projLoc = document.createElement("div");

    //projLoc.className = "proj-loc";
    //projLoc.textContent = "Set Project location here or drag pin to desired location";
    projLoc.innerHTML = '<span class="material-symbols-outlined">distance</span>';

    const faPin = new PinElement({
        glyph: projLoc,
        glyphColor: "#ff8300",
        background: "#FFD514",
        borderColor: "#ff8300",
      });

    //document.getElementById(outValue).value = myLatlng.lat + "," + myLatlng.lng;

    //  var infoWindow = new google.maps.InfoWindow({ position: myLatlng });
    //  infoWindow.setContent(
    //      '<i class="fad fa-store"></i> ' + 'aa' + " <br/> <a target='_blank' style='margin-left: 19.5px;text-decoration: none;' href='https://www.google.com/maps/place/aa/@" + defLat + "," + defLong + ",15z'>View on Google Maps</a>"
    //  );

    //  var oMarker = new google.maps.Marker({
    //      map: map,
    //      icon: {},
    //      title: "Project Location",
    //      position: myLatlng,
    //  });

    //  infoWindow.open(map, oMarker);

    const input = document.getElementById(searchElem);
    const searchBox = new google.maps.places.SearchBox(input);
    var outputLat = document.getElementById('outputLat');
    var outputLng = document.getElementById('outputLng');
    var outputaddress = document.getElementById('outputaddress');
    var outputpluscode = document.getElementById('outputpluscode');

    /*map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);*/

    map.addListener("bounds_changed", () => {
        searchBox.setBounds(map.getBounds());
    });

    var geocoder = new google.maps.Geocoder();
    const  draggableMarker =  new AdvancedMarkerElement({
        map,
        position: myLatlng,
        gmpDraggable: true,
    });

    draggableMarker.addListener("dragend", (event) => {
        const position = draggableMarker.position;

        searchLatLng = {lat:position.lat, lng: position.lng};
        infoWindow.close();
        infoWindow.open(draggableMarker.map, draggableMarker);

        geocoder.geocode({ 'latLng': searchLatLng }, (results, status) => {
            // This is checking to see if the Geoeode Status is OK before proceeding
            if (status == google.maps.GeocoderStatus.OK) {
                let splitAddress = results[0].formatted_address.split(' ');
                let uniqueArray = [...new Set(splitAddress)];

                outputpluscode.value = "";
                
                if (splitAddress[0].includes('+'))
                {
                    outputpluscode.value = splitAddress[0].replace(',', '');
                    outputaddress.value = uniqueArray.slice(1).join(' ');
                }
                else
                {
                    outputaddress.value = uniqueArray.slice(0).join(' ');
                }

            // Extracting the two parts
               
                outputLat.value = position.lat;
                outputLng.value = position.lng;

                var event = new Event('input', {
                    bubbles: true,
                });

                outputaddress.dispatchEvent(event);
                outputpluscode.dispatchEvent(event);
                outputLat.dispatchEvent(event);
                outputLng.dispatchEvent(event);
            }
        }); 

    });

    infoWindow.open(draggableMarker.map, draggableMarker);

    var markers = [];

    searchBox.addListener("places_changed", () => {
        const places = searchBox.getPlaces();

        if (places.length == 0) {
            return;
        }

        // Clear out the old markers.
        markers.forEach((marker) => {
            marker.setMap(null);
        });
        markers = [];

        // For each place, get the icon, name and location.
        const bounds = new google.maps.LatLngBounds();

        places.forEach((place) => {
            if (!place.geometry || !place.geometry.location) {
                console.log("Returned place contains no geometry");
                return;
            }

            if (place.geometry.viewport) {
                // Only geocodes have viewport.
                bounds.union(place.geometry.viewport);
            } else {
                bounds.extend(place.geometry.location);
            }
        });
        map.fitBounds(bounds);

        geocoder.geocode({
          'address': input.value
            }, function(results, status) {
      
            if (status == google.maps.GeocoderStatus.OK) {
                var latitude = results[0].geometry.location.lat();
                var longitude = results[0].geometry.location.lng();
                searchLatLng = {lat:latitude, lng: longitude};


                // if (searchMarker != null)
                // {
                //     searchMarker.setMap(null);
                // }

                // searchMarker = new google.maps.Marker({
                //     map: map,
                //     icon: {},
                //     title: "Set Project location",
                //     position: searchLatLng
                // });

                 
        
                // infoWindow.open(map, searchMarker);

                //const infoWindow = new google.maps.InfoWindow({ position: searchLatLng });
              
                draggableMarker.position = searchLatLng;
                
                map.setCenter(searchLatLng);

                geocoder.geocode({ 'latLng': searchLatLng }, (results, status) => {
                    // This is checking to see if the Geoeode Status is OK before proceeding
                    if (status == google.maps.GeocoderStatus.OK) {
                       
                        let splitAddress = results[0].formatted_address.split(' ');
                        let uniqueArray = [...new Set(splitAddress)];
                        outputpluscode.value= "";

                        if (splitAddress[0].includes('+'))
                        {
                            outputpluscode.value = splitAddress[0].replace(',', '');
                            outputaddress.value = uniqueArray.slice(1).join(' ');
                        }
                        else
                        {
                            outputaddress.value = uniqueArray.slice(0).join(' ');
                        }

                        outputLat.value = latitude;
                        outputLng.value = longitude;

                        var event = new Event('input', {
                            bubbles: true,
                        });

                        outputaddress.dispatchEvent(event);
                        outputpluscode.dispatchEvent(event);
                        outputLat.dispatchEvent(event);
                        outputLng.dispatchEvent(event); 
                    }
                }); 
            }
        });
    });

    // Configure the click listener.
    map.addListener("click", (mapsMouseEvent) => {
        // Close the current InfoWindow.
        infoWindow.close();

        // Create a new InfoWindow.

        draggableMarker.position = {lat: mapsMouseEvent.latLng.lat(), lng: mapsMouseEvent.latLng.lng()}

        infoWindow.open(draggableMarker.map, draggableMarker);
        
        geocoder.geocode({ 'latLng': {lat: mapsMouseEvent.latLng.lat(), lng: mapsMouseEvent.latLng.lng()} }, (results, status) => {
            // This is checking to see if the Geoeode Status is OK before proceeding
            if (status == google.maps.GeocoderStatus.OK) {
               
                let splitAddress = results[0].formatted_address.split(' ');
                let uniqueArray = [...new Set(splitAddress)];
                outputpluscode.value = "";

                if (splitAddress[0].includes('+'))
                {
                    outputpluscode.value = splitAddress[0].replace(',', '');
                    

                    outputaddress.value = uniqueArray.slice(1).join(' ');
                }
                else
                {
                    outputaddress.value = uniqueArray.slice(0).join(' ');
                }

                // Extracting the two parts
               
                outputLat.value = mapsMouseEvent.latLng.lat();
                outputLng.value = mapsMouseEvent.latLng.lng();

                var event = new Event('input', {
                    bubbles: true,
                });

                outputaddress.dispatchEvent(event);
                outputpluscode.dispatchEvent(event);
                outputLat.dispatchEvent(event);
                outputLng.dispatchEvent(event); 
            }
        }); 

    });
}

function getLatLongValue(outValueID) {
    return document.getElementById(outValueID).value;
}

async function getReverseGeocodingData(lat, lng) {
    return new Promise(resolve => {
        var latlng = new google.maps.LatLng(lat, lng);
        // This is making the Geocode request
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'latLng': latlng }, (results, status) => {
            if (status !== google.maps.GeocoderStatus.OK) {
                resolve('');
            }
            // This is checking to see if the Geoeode Status is OK before proceeding
            if (status == google.maps.GeocoderStatus.OK) {
                var address = (results[0].formatted_address);
                resolve(address);
            }
        }); 
    });
}

async function GetLatlong(searchBox, map) {
    
    var geocoder = new google.maps.Geocoder();
    var address = searchBox;//document.getElementById(searchBox).value;
    var latlng = "";
    geocoder.geocode({
      'address': address
    }, function(results, status) {
  
      if (status == google.maps.GeocoderStatus.OK) {
        var latitude = results[0].geometry.location.lat();
        var longitude = results[0].geometry.location.lng();
        searchLatLng = {lat:latitude, lng: longitude};

        
        searchMarker = new google.maps.Marker({
            map: map,
            icon: {},
            title: "Set Project location here, or drag pin to desired location",
            position: searchLatLng
        });

        var infoWindow = new google.maps.InfoWindow({ position: searchLatLng });
            infoWindow.setContent(
                'Set Project location here, or drag pin to desired location'
            );
   
        infoWindow.open(map, searchMarker);
        map.center = searchLatLng;

      }
    });

  }
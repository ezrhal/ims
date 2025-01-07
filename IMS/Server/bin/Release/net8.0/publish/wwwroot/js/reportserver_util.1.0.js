var serverHost = "http://192.168.1.72:83/";   
var serverTokenKey = "TelerikReportServerToken";  

  function login(username, password) {

    var accessToken = "";

    var xhr = new XMLHttpRequest();
    var url = serverHost + "Token";
    var params = "grant_type=password&username=" + encodeURIComponent(username) + "&password=" + encodeURIComponent(password);

    xhr.open("POST", url, false);
    xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded");

    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            if (xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                accessToken = data.access_token;
            } else {
                window.alert(xhr.status + ": " + xhr.statusText);
            }
        }
    };

    xhr.send(params);

    return accessToken;
  }



  function GetToken(a, b)
  {
    var accessToken = login(a, b);
    window.sessionStorage.setItem(serverTokenKey, accessToken);

  }

function GetReports() {
    return new Promise((resolve, reject) => {
        var token = window.sessionStorage.getItem(serverTokenKey);
        var headers = {};

        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        var serverApi = serverHost + "api/reportserver/v2/";

        var xhr = new XMLHttpRequest();
        var url = serverApi + "reports";
        xhr.open("GET", url, true);
        xhr.setRequestHeader("Content-type", "application/json");
        xhr.setRequestHeader("Authorization", headers.Authorization); // Assuming headers is an object with the Authorization property

        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                if (xhr.status == 200) {
                    var reports = JSON.parse(xhr.responseText);
                    resolve(reports);
                } else {
                    reject({ status: xhr.status, statusText: xhr.statusText });
                }
            }
        };

        xhr.send();
    });
}

function GetData(apicall) {
    return new Promise((resolve, reject) => {
        var token = window.sessionStorage.getItem(serverTokenKey);
        var headers = {};

        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        var serverApi = serverHost + "api/reportserver/v2/";

        var xhr = new XMLHttpRequest();
        var url = serverApi + apicall;
        xhr.open("GET", url, true);
        xhr.setRequestHeader("Content-type", "application/json");
        xhr.setRequestHeader("Authorization", headers.Authorization); // Assuming headers is an object with the Authorization property

        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                if (xhr.status == 200) {
                    var reports = JSON.parse(xhr.responseText);
                    resolve(reports);
                } else {
                    reject({ status: xhr.status, statusText: xhr.statusText });
                }
            }
        };

        xhr.send();
    });
}


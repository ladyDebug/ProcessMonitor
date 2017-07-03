**HTTP Performance check**
----
  Retreives the processes' data like memory used and process names.

* **URL**
 Currently it resides on 
  
http://localhost:8080/info

* **Method:**
   
  `GET` 
  
*  **URL Params**

   none

   **Required:**
 
   none

   **Optional:**
 
    None

* **Data Params**

    None

* **Success Response:**

Returns the json array which contains the processes' information.

  
  * **Code:** 200 <br />
    **Content:** 

`{"Process":[{"Id":860,"ProcessName":"svchost","WorkingSet":21409792},{"Id":7972,"ProcessName":"ScriptedSandbox64","WorkingSet":37142528},{"Id":5628,"ProcessName":"NvStreamNetworkService","WorkingSet":10588160},{"Id":3436,"ProcessName":"Memory Compression","WorkingSet":806449152},{"Id":3004,"ProcessName":"esif_uf","WorkingSet":5521408}]}`


* **Error Response:**

  * **Code:** 404 NOT FOUND <br />
  * **Code:** 500 SERVER ERROR <br />
    

* **Sample Call:**

```
$.getJSON("http://localhost:8080/info/", function (data) {
                $("#tbody").children().remove();
                for (var i = 0; i < data.Process.length; i++) {
                    var counter = data.Process[i];
                    var eachrow = "<tr>"
                        + "<td>" + counter.Id + "</td>"
                        + "<td>" + counter.ProcessName + "</td>"
                        + "<td>" + counter.WorkingSet/1024 + "</td>"
                        + "</tr>";
                    $("#tbody").append(eachrow);
                }
            }
            );
        }
``` 


* **Notes:**

Just start the index.html it will run the ajax query to the localhost:8080/info as it can be seen in the sample above.


  
**WEBSOCKET Events**
----
  Notifies the websocket clients the RAM/CPU information in case of host's high-load. In our case if it runs below < 1024MB or higher than 90% of CPU usage.

* **URL**
  Web socket  
  `ws://localhost:8080/`
* **Event format**
ram and cpu values are floats
`{"Ram":"1234","Cpu":"123"}

* **Notes**

You can see the sample ws connect in the attached html  



## How to re-create the bug

1. Run `make start-web-locally` - that will start the app
2. Execute the request in the file [requests.http](requests/requests.http)
3. Execute the request again and again, but wait for it to finish between each request. It will eventually fail because on the requests before the failed one the coordinator actor didn't stop the way it should.


# Sample Stream Snippets API(s)
Simple consolidated set of APIs/services to mimic functionality needed to support ingesting snippets (tweets, posts, stories, other social like text snippets that may include a body, tags, mentions) and calculate various statistics over the set of data. Currently using tech including Redis, Kafka, Prometheus for actually concrete implemenations of services.

## [Clone repo](#clone-repo)

```bash
git clone https://github.com/boydc7/Samples.Stream
cd Samples.Stream
```

## [Docker Run](#docker-run)
To run the entire stack in Docker (inlcuding the API), simply docker compose up:

```bash
docker compose up
```

NOTE: To rebuild and force recreation of the various containers:
```bash
docker compose up --build --force-recreate --renew-anon-volumes
```
## [Docker Run Helper](#docker-run-helper)
A simple bash script is also included to help with running the stack in various states.

```bash
bash docker-up-local.sh
```

TO RESET LOCAL ENV (including a rebuild):
```bash
bash docker-up-local.sh reset
```

TO START THE API (in addition to the default of just starting dependencies):
```bash
bash docker-up-local.sh api
```

TO START THE API AND REBUILD/RESET LOCAL ENV
```bash
bash docker-up-local.sh reset api
```

TO START THE API AND REBUILD CODE ONLY (without resetting local dependencies):
```bash
bash docker-up-local.sh build api
```

## [Docker Stop](#docker-stop)

To stop the stack:
```bash
docker compose -f docker-compose.yml down
```

## [Swagger](#swagger)
A Swagger doc for the API is available at the root, simply open the following url to inspect:

http://localhost:8082

There are 2 endpoints for use - a POST to send snippets into the system, a GET to retrieve current statistics about snippets processed.

## [Postman](#postman)
A [Postman](https://www.postman.com/) collection is included in the root of the project (see the [postman_collection.json](/postman_collection.json) file in the root of the repo) for use with exercising the various API endpoints as you like manually or in an automated fashion.

## [Prometheus](#prometheus)
A [Prometheus](https://prometheus.io/) instance is included in the docker compose setup along with metric collection and delivery within the services. You can inspect the various metrics (and graph them over time) via the included Prometheus webui accessible at http://localhost:29090.  The 3 metrics of most interest would be:

* stream_api_snippets_posted - the number of snippets posted to the API
* stream_ingestion_snippets_produced - the number of snippets ingested into the ingestion pipeline (q'd for processing)
* stream_processing_snippets_processed - the number of snippets processed over the stats processing service(s)

# [NOTES](#notes)
For the sake of time and simplicity, I've made the following assumptions and/or taken the following shortcuts:

* Very little error/exception handling and logging
* Minimal instrumentation, resilience, fault handling, etc.
* Assume that missing/dropping some snippets due to infrequent total failures is acceptable

# [LOAD TESTING](#load-testing)
I did not have much success finding/consuming a Reddit endpoint (most seemed geared toward a single thread, not all threads, but I didn't spend a lot of time investigating). As such, I simply instead created a simple console application that will send large-ish volumes of snippets into the POST api endpoint to send through the system.

A build of the simple console app is in the ./lib/macload folder.

TO LOAD TEST the api with batch posts of 500 snippets in each post with a 250 milliseond delay between each batch post:
```bash
./lib/macload/sload 500 250
```

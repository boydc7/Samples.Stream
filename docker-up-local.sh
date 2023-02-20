#!/usr/bin/env bash

# USAGE
#	bash docker-up-local.sh
#
#	TO RESET LOCAL ENV (including a rebuild):
#		bash docker-up-local.sh reset
#
#	TO START THE API (in addition to the default of just starting dependencies):
#		bash docker-up-local.sh api
#
#	TO START THE API AND REBUILD/RESET LOCAL ENV
#		bash docker-up-local.sh reset api
#
#	TO START THE API AND REBUILD CODE ONLY (without resetting local dependencies):
#		bash docker-up-local.sh build api
#
# 	To stop:
# 		docker-compose -f docker-compose.yml down
#

STREAM_COMPOSE_ARGS="--no-deps --detach"

if [ "$1" = "reset" ];
then
	echo "Cleaning dependencies and rebuilding..."
	STREAM_COMPOSE_ARGS="$STREAM_COMPOSE_ARGS --force-recreate --renew-anon-volumes --build"
elif [ "$1" = "build" ];
then
	echo "Rebuilding..."
	STREAM_COMPOSE_ARGS="$STREAM_COMPOSE_ARGS --build"
else 
	echo "Starting dependencies..." 
fi

docker compose -f docker-compose.yml up $STREAM_COMPOSE_ARGS stream-zookeeper stream-kafka stream-redis stream-prometheus

echo "STREAM dependency services are ready..."

if [ "$1" = "api" ] || [ "$2" = "api" ]; 
then
	echo "api specified, starting API..." 
	docker compose -f docker-compose.yml up $STREAM_COMPOSE_ARGS stream-api
	echo "API is ready..."
fi

echo ""
echo "All done"
echo ""

FROM couchdb:2.2.0
COPY local.ini /opt/couchdb/etc/
VOLUME ["/opt/couchdb/log"]
VOLUME ["/opt/couchdb/data"]
WORKDIR /opt/couchdb
EXPOSE 5984 4369 9100
ENTRYPOINT ["tini", "--", "/docker-entrypoint.sh"]
CMD ["/opt/couchdb/bin/couchdb"]
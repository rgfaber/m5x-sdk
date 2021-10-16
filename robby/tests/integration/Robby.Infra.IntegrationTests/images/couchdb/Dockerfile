FROM apache/couchdb
COPY --chown=couchdb:couchdb local.ini /etc/local.ini
VOLUME ["/opt/couchdb/var/log"]
VOLUME ["/opt/couchdb/data"]
WORKDIR /opt/couchdb
EXPOSE 5984 4369 9100
ENTRYPOINT ["tini", "--", "/docker-entrypoint.sh"]
CMD ["/opt/couchdb/bin/couchdb"]
FROM grafana/grafana:8.4.4-ubuntu

# Disable Login form or not
ENV GF_AUTH_DISABLE_LOGIN_FORM "false"
# Allow anonymous authentication or not
ENV GF_AUTH_ANONYMOUS_ENABLED "true"
# Role of anonymous user
ENV GF_AUTH_ANONYMOUS_ORG_ROLE "Viewer"
# Install plugins here our in your own config file
# ENV GF_INSTALL_PLUGINS="<list of plugins seperated by ,"

# Add provisioning
ADD ./provisioning /etc/grafana/provisioning
# Add configuration file
ADD ./config.ini /etc/grafana/config.ini
# Add dashboard json files
ADD ./dashboards /var/lib/grafana/dashboards
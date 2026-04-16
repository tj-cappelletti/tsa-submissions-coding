#!/bin/sh
set -eu

cat <<EOF >/usr/share/nginx/html/env-config.js
window.__API_URL_CONFIG__ = "${API_URL:-}";
EOF

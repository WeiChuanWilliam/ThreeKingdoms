#!/usr/bin/env bash
# Ensure every Assets/**/*.cs has a sibling .cs.meta; report orphan .cs.meta files.
set -euo pipefail

root="$(cd "$(dirname "$0")/.." && pwd)"
created=0
orphans=0

while IFS= read -r cs; do
  meta="${cs}.meta"
  if [[ ! -f "$meta" ]]; then
    guid="$(uuidgen | tr '[:upper:]' '[:lower:]' | tr -d '-')"
    cat >"$meta" <<EOF
fileFormatVersion: 2
guid: ${guid}
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF
    echo "CREATED $meta"
    created=$((created + 1))
  fi
done < <(find "$root/Assets" -name '*.cs' | sort)

while IFS= read -r meta; do
  cs="${meta%.meta}"
  if [[ ! -f "$cs" ]]; then
    echo "ORPHAN $meta"
    orphans=$((orphans + 1))
  fi
done < <(find "$root/Assets" -name '*.cs.meta' | sort)

echo "Done. created=$created orphans=$orphans"

#!/bin/bash

GREEN='\033[0;32m'
YELLOW='\033[0;33m'
NC='\033[0m' 

echo -e "${YELLOW}Analyse de l'image Docker avec Trivy...${NC}"

# Créer le répertoire des rapports s'il n'existe pas
mkdir -p rapports

# Analyser l'image Docker
docker run --rm -v /var/run/docker.sock:/var/run/docker.sock \
  -v "$(pwd)/rapports:/reports" \
  aquasec/trivy:latest image \
  --format json \
  --output /reports/trivy-image.json \
  ecf-dotnet-app:latest

# Générer également un rapport HTML pour une meilleure lisibilité
docker run --rm -v /var/run/docker.sock:/var/run/docker.sock \
  -v "$(pwd)/rapports:/reports" \
  aquasec/trivy:latest image \
  --format template \
  --template "@/contrib/html.tpl" \
  --output /reports/trivy-image-report.html \
  ecf-dotnet-app:latest

# Afficher un résumé des vulnérabilités trouvées
echo -e "${YELLOW}Résumé des vulnérabilités trouvées :${NC}"
docker run --rm -v /var/run/docker.sock:/var/run/docker.sock \
  aquasec/trivy:latest image \
  --severity HIGH,CRITICAL \
  ecf-dotnet-app:latest

echo -e "${GREEN}=== Analyse de l'image Docker terminée ===${NC}"
echo -e "Les rapports sont disponibles dans le répertoire 'rapports':"
ls -la rapports/trivy-image*

echo -e "\nPour consulter le rapport HTML:"
echo "open rapports/trivy-image-report.html"

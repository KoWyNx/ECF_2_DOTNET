#!/bin/bash

VERT='\033[0;32m'
ROUGE='\033[0;31m'
JAUNE='\033[0;33m'
BLEU='\033[0;34m'
NC='\033[0m'

# Nom de l'image Docker à analyser
IMAGE_NAME="ecf_2_dotnet-app:latest"

echo -e "${BLEU}=== Démarrage de l'analyse de l'image Docker avec Trivy ===${NC}"

# Créer le répertoire des rapports s'il n'existe pas
mkdir -p rapports

# Vérifier si Trivy est disponible via Docker
if ! docker info > /dev/null 2>&1; then
    echo -e "${ROUGE}Docker n'est pas disponible. Impossible d'exécuter Trivy.${NC}"
    exit 1
fi

# Vérifier si l'image existe
if ! docker image inspect ${IMAGE_NAME} > /dev/null 2>&1; then
    echo -e "${JAUNE}L'image ${IMAGE_NAME} n'existe pas. Tentative de construction...${NC}"
    docker-compose build app
    
    if [ $? -ne 0 ]; then
        echo -e "${ROUGE}Échec de la construction de l'image. Veuillez la construire manuellement.${NC}"
        exit 1
    fi
fi

echo -e "${JAUNE}Analyse de l'image Docker avec Trivy...${NC}"

# Analyse de l'image Docker en format table pour avoir un résumé lisible
docker run --rm -v /var/run/docker.sock:/var/run/docker.sock \
  -v "$(pwd)/rapports:/reports" \
  aquasec/trivy:latest image \
  --format table \
  --output /reports/trivy-image-summary.txt \
  ${IMAGE_NAME}

# Générer également un rapport JSON
docker run --rm -v /var/run/docker.sock:/var/run/docker.sock \
  -v "$(pwd)/rapports:/reports" \
  aquasec/trivy:latest image \
  --format json \
  --output /reports/trivy-image.json \
  ${IMAGE_NAME}

# Générer également un rapport HTML 
docker run --rm -v /var/run/docker.sock:/var/run/docker.sock \
  -v "$(pwd)/rapports:/reports" \
  aquasec/trivy:latest image \
  --format template \
  --template "@/contrib/html.tpl" \
  --output /reports/trivy-image-report.html \
  ${IMAGE_NAME}

# Créer un résumé simplifié pour la présentation
echo "=== RÉSUMÉ DE L'ANALYSE TRIVY DE L'IMAGE DOCKER ===" > rapports/trivy-image-resume.txt
echo "Date: $(date '+%d/%m/%Y %H:%M:%S')" >> rapports/trivy-image-resume.txt
echo "Image analysée: ${IMAGE_NAME}" >> rapports/trivy-image-resume.txt
echo "" >> rapports/trivy-image-resume.txt

# Extraire et compter les vulnérabilités par gravité
echo "STATISTIQUES DES VULNÉRABILITÉS:" >> rapports/trivy-image-resume.txt
docker run --rm \
  -v "$(pwd)/rapports:/reports" \
  --entrypoint sh \
  aquasec/trivy:latest \
  -c "cat /reports/trivy-image.json | grep -o '\"Severity\":\"[^\"]*\"' | sort | uniq -c | sed 's/\"Severity\":\"/ /g' | sed 's/\"//g'" >> rapports/trivy-image-resume.txt

echo "" >> rapports/trivy-image-resume.txt
echo "VULNÉRABILITÉS CRITIQUES ET ÉLEVÉES:" >> rapports/trivy-image-resume.txt
echo "Pour les détails des vulnérabilités critiques et élevées, veuillez consulter:" >> rapports/trivy-image-resume.txt
echo "- rapports/trivy-image-summary.txt (format texte)" >> rapports/trivy-image-resume.txt
echo "- rapports/trivy-image-report.html (format HTML)" >> rapports/trivy-image-resume.txt

echo "" >> rapports/trivy-image-resume.txt
echo "RECOMMANDATIONS:" >> rapports/trivy-image-resume.txt
echo "1. Mettre à jour les packages système avec des vulnérabilités CRITICAL en priorité" >> rapports/trivy-image-resume.txt
echo "2. Planifier la mise à jour des packages avec des vulnérabilités HIGH" >> rapports/trivy-image-resume.txt
echo "3. Utiliser une image de base plus récente si possible" >> rapports/trivy-image-resume.txt
echo "" >> rapports/trivy-image-resume.txt
echo "Pour plus de détails, voir les rapports complets:" >> rapports/trivy-image-resume.txt
echo "- rapports/trivy-image-summary.txt (format texte)" >> rapports/trivy-image-resume.txt
echo "- rapports/trivy-image-report.html (format HTML)" >> rapports/trivy-image-resume.txt
echo "- rapports/trivy-image.json (format JSON)" >> rapports/trivy-image-resume.txt

# Extraire les 20 premières lignes du rapport texte pour avoir un aperçu
echo "" >> rapports/trivy-image-resume.txt
echo "APERÇU DU RAPPORT:" >> rapports/trivy-image-resume.txt
echo "-------------------" >> rapports/trivy-image-resume.txt
docker run --rm \
  -v "$(pwd)/rapports:/reports" \
  --entrypoint sh \
  aquasec/trivy:latest \
  -c "head -20 /reports/trivy-image-summary.txt" >> rapports/trivy-image-resume.txt
echo "..." >> rapports/trivy-image-resume.txt

if [ $? -eq 0 ]; then
    echo -e "${VERT}✓ Analyse de l'image Docker terminée avec succès${NC}"
    echo -e "${VERT}✓ Rapports enregistrés dans:${NC}"
    echo -e "${VERT}  - rapports/trivy-image.json (JSON complet)${NC}"
    echo -e "${VERT}  - rapports/trivy-image-report.html (HTML formaté)${NC}"
    echo -e "${VERT}  - rapports/trivy-image-summary.txt (Texte tabulaire)${NC}"
    echo -e "${VERT}  - rapports/trivy-image-resume.txt (Résumé simplifié)${NC}"
else
    echo -e "${ROUGE}✗ Échec de l'analyse de l'image Docker${NC}"
fi

echo -e "\n${BLEU}=== Analyse de l'image Docker terminée ===${NC}"

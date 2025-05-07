#!/bin/bash

VERT='\033[0;32m'
ROUGE='\033[0;31m'
JAUNE='\033[0;33m'
BLEU='\033[0;34m'
NC='\033[0m'

echo -e "${BLEU}=== Démarrage de l'analyse des dépendances avec Trivy ===${NC}"

# Créer le répertoire des rapports s'il n'existe pas
mkdir -p rapports

# Vérifier si Trivy est disponible via Docker
if ! docker info > /dev/null 2>&1; then
    echo -e "${ROUGE}Docker n'est pas disponible. Impossible d'exécuter Trivy.${NC}"
    exit 1
fi

echo -e "${JAUNE}Analyse des dépendances du projet .NET...${NC}"

# Analyse des dépendances avec Trivy en format table pour avoir un résumé lisible
docker run --rm \
  -v "$(pwd):/app" \
  -v "$(pwd)/rapports:/reports" \
  aquasec/trivy:latest fs \
  --format table \
  --output /reports/trivy-deps-summary.txt \
  /app

# Générer également un rapport JSON
docker run --rm \
  -v "$(pwd):/app" \
  -v "$(pwd)/rapports:/reports" \
  aquasec/trivy:latest fs \
  --format json \
  --output /reports/trivy-deps.json \
  /app

# Générer également un rapport HTML
docker run --rm \
  -v "$(pwd):/app" \
  -v "$(pwd)/rapports:/reports" \
  aquasec/trivy:latest fs \
  --format template \
  --template "@/contrib/html.tpl" \
  --output /reports/trivy-deps.html \
  /app

# Créer un résumé simplifié pour la présentation
echo "=== RÉSUMÉ DE L'ANALYSE TRIVY DES DÉPENDANCES ===" > rapports/trivy-resume.txt
echo "Date: $(date '+%d/%m/%Y %H:%M:%S')" >> rapports/trivy-resume.txt
echo "" >> rapports/trivy-resume.txt

# Extraire et compter les vulnérabilités par gravité
echo "STATISTIQUES DES VULNÉRABILITÉS:" >> rapports/trivy-resume.txt
docker run --rm \
  -v "$(pwd)/rapports:/reports" \
  --entrypoint sh \
  aquasec/trivy:latest \
  -c "cat /reports/trivy-deps.json | grep -o '\"Severity\":\"[^\"]*\"' | sort | uniq -c | sed 's/\"Severity\":\"/ /g' | sed 's/\"//g'" >> rapports/trivy-resume.txt

echo "" >> rapports/trivy-resume.txt
echo "VULNÉRABILITÉS CRITIQUES ET ÉLEVÉES:" >> rapports/trivy-resume.txt
echo "Pour les détails des vulnérabilités critiques et élevées, veuillez consulter:" >> rapports/trivy-resume.txt
echo "- rapports/trivy-deps-summary.txt (format texte)" >> rapports/trivy-resume.txt
echo "- rapports/trivy-deps.html (format HTML)" >> rapports/trivy-resume.txt

echo "" >> rapports/trivy-resume.txt
echo "RECOMMANDATIONS:" >> rapports/trivy-resume.txt
echo "1. Mettre à jour les packages avec des vulnérabilités CRITICAL en priorité" >> rapports/trivy-resume.txt
echo "2. Planifier la mise à jour des packages avec des vulnérabilités HIGH" >> rapports/trivy-resume.txt
echo "3. Consulter le rapport complet pour plus de détails" >> rapports/trivy-resume.txt
echo "" >> rapports/trivy-resume.txt
echo "Pour plus de détails, voir les rapports complets:" >> rapports/trivy-resume.txt
echo "- rapports/trivy-deps-summary.txt (format texte)" >> rapports/trivy-resume.txt
echo "- rapports/trivy-deps.html (format HTML)" >> rapports/trivy-resume.txt
echo "- rapports/trivy-deps.json (format JSON)" >> rapports/trivy-resume.txt

# Extraire les 10 premières lignes du rapport texte pour avoir un aperçu
echo "" >> rapports/trivy-resume.txt
echo "APERÇU DU RAPPORT:" >> rapports/trivy-resume.txt
echo "-------------------" >> rapports/trivy-resume.txt
docker run --rm \
  -v "$(pwd)/rapports:/reports" \
  --entrypoint sh \
  aquasec/trivy:latest \
  -c "head -20 /reports/trivy-deps-summary.txt" >> rapports/trivy-resume.txt
echo "..." >> rapports/trivy-resume.txt

if [ $? -eq 0 ]; then
    echo -e "${VERT}✓ Analyse des dépendances terminée avec succès${NC}"
    echo -e "${VERT}✓ Rapports enregistrés dans:${NC}"
    echo -e "${VERT}  - rapports/trivy-deps.json (JSON complet)${NC}"
    echo -e "${VERT}  - rapports/trivy-deps.html (HTML formaté)${NC}"
    echo -e "${VERT}  - rapports/trivy-deps-summary.txt (Texte tabulaire)${NC}"
    echo -e "${VERT}  - rapports/trivy-resume.txt (Résumé simplifié)${NC}"
else
    echo -e "${ROUGE}✗ Échec de l'analyse des dépendances${NC}"
fi

echo -e "\n${BLEU}=== Analyse des dépendances terminée ===${NC}"

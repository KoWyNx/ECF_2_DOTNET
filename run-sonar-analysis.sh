#!/bin/bash

VERT='\033[0;32m'
ROUGE='\033[0;31m'
JAUNE='\033[0;33m'
BLEU='\033[0;34m'
NC='\033[0m'

echo -e "${BLEU}=== Démarrage de l'analyse SonarQube ===${NC}"

# Création du répertoire de rapports
mkdir -p rapports

# Vérifier si dotnet-sonarscanner est installé
echo -e "\n${JAUNE}Vérification de dotnet-sonarscanner...${NC}"
if ! dotnet tool list -g | grep -q "dotnet-sonarscanner"; then
    echo -e "${JAUNE}Installation de dotnet-sonarscanner...${NC}"
    dotnet tool install --global dotnet-sonarscanner
    
    if [ $? -ne 0 ]; then
        echo -e "${ROUGE}Échec de l'installation de dotnet-sonarscanner. Veuillez l'installer manuellement.${NC}"
        exit 1
    fi
fi

# Vérifier si le token SonarQube est défini
if [ -z "$SONAR_TOKEN" ]; then
    echo -e "${JAUNE}Variable d'environnement SONAR_TOKEN non définie.${NC}"
    echo -e "${JAUNE}Utilisation de l'authentification anonyme (pour les instances locales uniquement).${NC}"
    
    # Définir l'URL de SonarQube (par défaut: localhost)
    SONAR_HOST_URL=${SONAR_HOST_URL:-"http://localhost:9000"}
    
else
    SONAR_HOST_URL=${SONAR_HOST_URL:-"http://localhost:9000"}
    echo -e "${VERT}Configuration SonarQube trouvée.${NC}"
    echo -e "${JAUNE}URL SonarQube: ${SONAR_HOST_URL}${NC}"
fi

# Nettoyer les analyses précédentes
echo -e "\n${JAUNE}Nettoyage des analyses précédentes...${NC}"
rm -rf .sonarqube

# Exécuter les tests avec couverture de code
echo -e "\n${JAUNE}Exécution des tests avec couverture de code...${NC}"
dotnet test EcfDotnet.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

if [ $? -ne 0 ]; then
    echo -e "${ROUGE}Échec des tests unitaires${NC}"
    echo -e "${JAUNE}L'analyse SonarQube continuera mais pourrait être incomplète.${NC}"
fi

# Démarrer l'analyse SonarQube
echo -e "\n${JAUNE}Démarrage de l'analyse SonarQube...${NC}"

# Étape Begin
if [ -n "$SONAR_TOKEN" ]; then
    # Avec authentification
    dotnet sonarscanner begin \
      /k:"ECF_2_DOTNET" \
      /n:"ECF_2_DOTNET" \
      /d:sonar.host.url="${SONAR_HOST_URL}" \
      /d:sonar.login="${SONAR_TOKEN}" \
      /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" \
      /d:sonar.coverage.exclusions="**Test*.cs" \
      /d:sonar.sourceEncoding="UTF-8"
else
    # Sans authentification (pour les instances locales)
    dotnet sonarscanner begin \
      /k:"ECF_2_DOTNET" \
      /n:"ECF_2_DOTNET" \
      /d:sonar.host.url="${SONAR_HOST_URL}" \
      /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" \
      /d:sonar.coverage.exclusions="**Test*.cs" \
      /d:sonar.sourceEncoding="UTF-8"
fi

# Compiler le projet
echo -e "\n${JAUNE}Compilation du projet...${NC}"
dotnet build EcfDotnet.csproj --no-incremental

# Étape End
echo -e "\n${JAUNE}Finalisation de l'analyse SonarQube...${NC}"
if [ -n "$SONAR_TOKEN" ]; then
    dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"
else
    dotnet sonarscanner end
fi

# Vérifier si l'analyse a réussi
if [ $? -eq 0 ]; then
    echo -e "${VERT}✓ Analyse SonarQube terminée avec succès${NC}"
    
    # Créer un fichier de rapport simple pour référence
    cat > rapports/sonar-analysis-info.txt << EOL
Analyse SonarQube
=================
Date: $(date '+%d/%m/%Y %H:%M:%S')
Projet: ECF_2_DOTNET
URL des résultats: ${SONAR_HOST_URL}/dashboard?id=ECF_2_DOTNET

Pour voir les résultats complets, accédez à l'URL ci-dessus.
EOL

    echo -e "${VERT}✓ Informations de l'analyse enregistrées dans: rapports/sonar-analysis-info.txt${NC}"
else
    echo -e "${ROUGE}✗ Échec de l'analyse SonarQube${NC}"
fi

echo -e "\n${BLEU}=== Analyse SonarQube terminée ===${NC}"
#!/bin/bash

# Définition des couleurs pour l'affichage
VERT='\033[0;32m'
ROUGE='\033[0;31m'
JAUNE='\033[0;33m'
BLEU='\033[0;34m'
NC='\033[0m' 

echo -e "${BLEU}=== Démarrage de l'analyse SonarQube ===${NC}"

# Création du répertoire de rapports
mkdir -p rapports

# Vérifier si dotnet-sonarscanner est installé
if ! dotnet tool list -g | grep -q "dotnet-sonarscanner"; then
    echo -e "${JAUNE}Installation de dotnet-sonarscanner...${NC}"
    dotnet tool install --global dotnet-sonarscanner
    
    if [ $? -ne 0 ]; then
        echo -e "${ROUGE}Échec de l'installation de dotnet-sonarscanner${NC}"
        exit 1
    fi
fi

# Vérifier si le token SonarQube est défini
if [ -z "$SONAR_TOKEN" ]; then
    echo -e "${JAUNE}Variable d'environnement SONAR_TOKEN non définie.${NC}"
    echo -e "${JAUNE}Utilisation d'une configuration locale pour l'analyse.${NC}"
    
    # Définir l'URL de SonarQube (par défaut: localhost)
    SONAR_HOST_URL=${SONAR_HOST_URL:-"http://localhost:9000"}
    
    echo -e "${JAUNE}Pour configurer SonarQube correctement, exécutez:${NC}"
    echo -e "export SONAR_TOKEN=votre_token"
    echo -e "export SONAR_HOST_URL=votre_url_sonarqube"
else
    SONAR_HOST_URL=${SONAR_HOST_URL:-"http://localhost:9000"}
    echo -e "${VERT}Configuration SonarQube trouvée.${NC}"
    echo -e "${JAUNE}URL SonarQube: ${SONAR_HOST_URL}${NC}"
fi

# Exécuter les tests avec couverture de code
echo -e "\n${JAUNE}Exécution des tests avec couverture de code...${NC}"
dotnet test EcfDotnet.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

if [ $? -ne 0 ]; then
    echo -e "${ROUGE}Échec des tests unitaires${NC}"
    echo -e "${JAUNE}L'analyse SonarQube continuera mais pourrait être incomplète.${NC}"
fi

# Démarrer l'analyse SonarQube
echo -e "\n${JAUNE}Démarrage de l'analyse SonarQube...${NC}"

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
echo -e "\n${JAUNE}Compilation du projet pour l'analyse...${NC}"
dotnet build EcfDotnet.csproj --no-incremental

# Terminer l'analyse SonarQube
echo -e "\n${JAUNE}Finalisation de l'analyse SonarQube...${NC}"

if [ -n "$SONAR_TOKEN" ]; then
    dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"
else
    dotnet sonarscanner end
fi

if [ $? -eq 0 ]; then
    echo -e "${VERT}✓ Analyse SonarQube terminée avec succès${NC}"
    echo -e "${JAUNE}Consultez les résultats sur: ${SONAR_HOST_URL}/dashboard?id=ECF_2_DOTNET${NC}"
    
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

BASE_PATH="./sde-static"
if [ ! -f "$BASE_PATH/invTypes.csv" ]; then
    mkdir -p "$BASE_PATH"
    curl https://www.fuzzwork.co.uk/dump/latest/invTypes.csv.bz2 --output "$BASE_PATH/invTypes.csv.bz2"
    bzip2 -d "$BASE_PATH/invTypes.csv.bz2"
fi
if [ ! -f "$BASE_PATH/industryActivityMaterials.csv" ]; then
    mkdir -p "$BASE_PATH"
    curl https://www.fuzzwork.co.uk/dump/latest/industryActivityMaterials.csv.bz2 --output "$BASE_PATH/industryActivityMaterials.csv.bz2"
    bzip2 -d "$BASE_PATH/industryActivityMaterials.csv.bz2"
fi
if [ ! -f "$BASE_PATH/industryActivityProducts.csv" ]; then
    mkdir -p "$BASE_PATH"
    curl https://www.fuzzwork.co.uk/dump/latest/industryActivityProducts.csv.bz2 --output "$BASE_PATH/industryActivityProducts.csv.bz2"
    bzip2 -d "$BASE_PATH/industryActivityProducts.csv.bz2"
fi

if [ ! -f "./sde/invTypes.csv" ]; then
    mkdir -p "./sde/"
    curl https://www.fuzzwork.co.uk/dump/latest/invTypes.csv.bz2 --output ./sde/invTypes.csv.bz2
    bzip2 -d "./sde/invTypes.csv.bz2"
fi
if [ ! -f "./sde/industryActivityMaterials.csv" ]; then
    mkdir -p "./sde/"
    curl https://www.fuzzwork.co.uk/dump/latest/industryActivityMaterials.csv.bz2 --output ./sde/industryActivityMaterials.csv.bz2
    bzip2 -d "./sde/industryActivityMaterials.csv.bz2"
fi
if [ ! -f "./sde/industryActivityProducts.csv" ]; then
    mkdir -p "./sde/"
    curl https://www.fuzzwork.co.uk/dump/latest/industryActivityProducts.csv.bz2 --output ./sde/industryActivityProducts.csv.bz2
    bzip2 -d "./sde/industryActivityProducts.csv.bz2"
fi

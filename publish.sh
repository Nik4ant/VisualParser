echo "THIS IS NOT WORKING YET"

if [[ "$OSTYPE" == "linux*"]]; then
    echo "Publish shit for linux on: $OSTYPE"
elif [[ "$OSTYPE" == "msys*" ]] || [[ "$OSTYPE" == "cygwin*"]]; then
    echo "No support for MacOS yet"
else
    echo "How you managed to run bash on this?"
fi

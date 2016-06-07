#!/usr/bin/env bash

[[ -n $GITHUB_TOKEN ]] || { echo >&2 "GITHUB_TOKEN variable is undefined, please provide token"; exit 1; }

# Authorizes with read-only access to GitHub API
curl_() {
    curl -s -i -H "Authorization: token $GITHUB_TOKEN" "$@"
}

# Retrieves asset ID and package name of asset ending in argument
# $info looks like: "id": 1698239, "name": "powershell_0.4.0-1_amd64.deb",
get_info() {
    curl_ https://api.github.com/repos/PowerShell/PowerShell/releases/latest | grep -B 1 "name.*$1"
}

# Get OS specific asset ID and package name
case "$OSTYPE" in
    linux*)
        # Install curl and wget to download package
        sudo apt-get install -y curl wget
        info=$(get_info deb)
        ;;
    darwin*)
        info=$(get_info pkg)
        ;;
    *)
        exit 2 >&2 "$OSTYPE not supported!"
        ;;
esac

# Parses $info for asset ID and package name
read asset package <<< $(echo $info | sed 's/[,"]//g' | awk '{ print $2; print $4 }')

# Downloads asset to file
curl_ -H 'Accept: application/octet-stream' https://api.github.com/repos/PowerShell/PowerShell/releases/assets/$asset |
    grep location | sed 's/location: //g' | wget -i - -O $package

# Installs PowerShell package
case "$OSTYPE" in
    linux*)
        # Install dependencies
        sudo apt-get install -y libunwind8 libicu52
        sudo dpkg -i ./$package
        ;;
    darwin*)
        sudo installer -pkg ./$package -target /
        ;;
esac

echo "Congratulations! PowerShell is now installed."

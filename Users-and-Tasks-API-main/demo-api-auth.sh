#!/bin/bash

# Configuration
API_BASE_URL="http://localhost:5000"
USERNAME="testuser_$(date +%s)"
PASSWORD="SecurePass123!"

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${YELLOW}=== API Authentication Demo ===${NC}"
echo "Base URL: $API_BASE_URL"
echo "Username: $USERNAME"
echo ""

# Function to make API requests
make_request() {
    local method=$1
    local endpoint=$2
    local data=$3
    local token=$4
    
    local cmd="curl -s -X $method \"$API_BASE_URL$endpoint\""
    
    if [ ! -z "$data" ]; then
        cmd+=" -H \"Content-Type: application/json\" -d '$data'"
    fi
    
    if [ ! -z "$token" ]; then
        cmd+=" -H \"Authorization: Bearer $token\""
    fi
    
    echo -e "\n${YELLOW}Request: $method $endpoint${NC}"
    if [ ! -z "$data" ]; then
        echo "Payload: $data"
    fi
    
    eval $cmd | jq .
    return $?
}

# 1. Register a new user
REGISTER_PAYLOAD="{\"username\":\"$USERNAME\",\"password\":\"$PASSWORD\",\"email\":\"$USERNAME@example.com\"}"
echo -e "${GREEN}1. Registering new user: $USERNAME${NC}"
make_request "POST" "/api/Auth/register" "$REGISTER_PAYLOAD"

# 2. Get access token
LOGIN_PAYLOAD="{\"username\":\"$USERNAME\",\"password\":\"$PASSWORD\"}"
echo -e "\n${GREEN}2. Getting access token${NC}"
TOKEN_RESPONSE=$(make_request "POST" "/api/Auth/login" "$LOGIN_PAYLOAD")
TOKEN=$(echo $TOKEN_RESPONSE | jq -r '.token' 2>/dev/null)

if [ -z "$TOKEN" ] || [ "$TOKEN" = "null" ]; then
    echo -e "\n❌ Failed to get access token. Response:"
    echo $TOKEN_RESPONSE
    exit 1
fi

echo -e "\nSuccessfully obtained access token!"

# 3. Use the token to access protected endpoints
echo -e "\n${GREEN}3. Testing protected endpoints with the token${NC}"

# Get current user profile
echo -e "\n🔍 Getting current user profile:"
make_request "GET" "/api/Users/me" "" "$TOKEN"

# Get all users
echo -e "\n👥 Getting all users:"
make_request "GET" "/api/Users" "" "$TOKEN"

echo -e "\n${GREEN} Authentication demo completed successfully!${NC}"
echo -e "\nYou can use this token for subsequent API requests:"
echo "Authorization: Bearer $TOKEN"

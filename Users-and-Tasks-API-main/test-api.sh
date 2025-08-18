#!/bin/bash

# Set the base URL
BASE_URL="http://localhost:5062"

# Test 1: Try to access a protected endpoint without a token
echo "\n=== Test 1: Access protected endpoint without token (should fail) ==="
curl -i -X GET "$BASE_URL/api/Users"

# Test 2: Register a new user
echo "\n\n=== Test 2: Register a new user ==="
REGISTER_RESPONSE=$(curl -s -X POST "$BASE_URL/api/Auth/register" \
  -H "Content-Type: application/json" \
  -d '{"username": "testuser", "password": "Test@123", "email": "test@example.com"}')
echo "Response: $REGISTER_RESPONSE"

# Test 3: Login with the new user
echo "\n=== Test 3: Login with the new user ==="
LOGIN_RESPONSE=$(curl -s -X POST "$BASE_URL/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "testuser", "password": "Test@123"}')
echo "Response: $LOGIN_RESPONSE"

# Extract the JWT token from the login response
TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"token":"[^"]*"' | cut -d'"' -f4)

# Test 4: Access protected endpoint with the token
echo "\n=== Test 4: Access protected endpoint with token (should succeed) ==="
curl -i -X GET "$BASE_URL/api/Users" \
  -H "Authorization: Bearer $TOKEN"

echo "\n=== Testing complete ==="

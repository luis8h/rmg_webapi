#!/bin/bash

# Define variable for username
username="postgres"

# Prompt the user to enter the password
read -s -p "Enter password for $username: " password
echo

liquibase update --url=jdbc:postgresql://192.168.188.179:5435/rmg_db --username="$username" --password="$password"

# 🐳 Docker Installation Guide

This guide will help you install **Docker** on your system. Docker is required to run infrastructure services like PostgreSQL and Azurite for the DistributedFileStorage project.

---

## 🔹 Windows

1. Download Docker Desktop:  
   👉 [https://www.docker.com/products/docker-desktop/](https://www.docker.com/products/docker-desktop/)

2. Run the installer and follow the on-screen instructions.

3. Restart your system if prompted.

4. Ensure **WSL 2** is enabled (Docker Desktop will guide you if not).

5. Verify Docker installation:

   ```bash
   docker --version
   docker-compose --version
   ```

---

## 🔹 macOS

1. Download Docker Desktop for macOS:  
   👉 [https://www.docker.com/products/docker-desktop/](https://www.docker.com/products/docker-desktop/)

2. Open the `.dmg` file and drag Docker to Applications.

3. Launch Docker from Applications.

4. Verify installation:

   ```bash
   docker --version
   docker-compose --version
   ```

---

## 🔹 Linux (Ubuntu / Debian)

```bash
# Remove any old Docker versions
sudo apt-get remove docker docker-engine docker.io containerd runc

# Update package index
sudo apt-get update

# Install prerequisites
sudo apt-get install -y ca-certificates curl gnupg lsb-release

# Add Docker’s GPG key
sudo mkdir -p /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | \
sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg

# Set up the stable repository
echo \
"deb [arch=$(dpkg --print-architecture) \
signed-by=/etc/apt/keyrings/docker.gpg] \
https://download.docker.com/linux/ubuntu \
$(lsb_release -cs) stable" | \
sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Install Docker Engine
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# Verify installation
docker --version
docker compose version

# Optional: Run Docker without sudo
sudo usermod -aG docker $USER
```

> Reboot or re-login required to apply group changes.

---

## ✅ Next Step

Once Docker is installed, run the following command from your project root to start infrastructure services:

```bash
docker-compose up -d
```
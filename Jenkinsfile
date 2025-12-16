pipeline {
    agent any
    
    stages {
        stage('Hello') {
            steps {
                echo 'Hello jenkins'
            }
        }
        
        stage('Check Structure') {
            steps {
                sh '''
                    echo "=== Current directory ==="
                    pwd
                    echo "=== Files in root ==="
                    ls -la
                    echo "=== Dockerfile content (first 10 lines) ==="
                    head -10 Dockerfile || echo "No Dockerfile"
                    echo "=== docker-compose.yml content ==="
                    cat docker-compose.yml || echo "No docker-compose.yml"
                '''
            }
        }
        
        stage('Build Docker Image') {
            steps {
                script {
                    // Убедитесь что docker-compose.yml существует
                    sh '''
                        if [ -f docker-compose.yml ]; then
                            docker compose build
                        else
                            echo "ERROR: docker-compose.yml not found!"
                            exit 1
                        fi
                    '''
                }
            }
        }
        
        stage('Start Docker Container') {
            steps {
                sh 'docker compose up -d'
            }
        }
        
        stage('Verify') {
            steps {
                sh '''
                    echo "=== Running containers ==="
                    docker ps
                    echo "=== Compose services ==="
                    docker compose ps
                    sleep 3
                    echo "=== Testing application ==="
                    curl -f http://localhost:5206 || echo "Application not responding"
                '''
            }
        }
    }
    
    post {
        always {
            sh 'docker compose logs --tail=50 web || true'
        }
    }
}

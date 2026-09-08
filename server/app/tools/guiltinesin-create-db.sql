DROP DATABASE IF EXISTS guiltinesin_local;
CREATE DATABASE guiltinesin_local DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
GRANT ALL PRIVILEGES ON guiltinesin_local.* TO 'guiltinesin'@'localhost' IDENTIFIED BY 'guiltinesin123';
FLUSH PRIVILEGES;

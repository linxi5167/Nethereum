﻿using System.Numerics;
using Nethereum.KeyStore;
using Nethereum.RPC.Accounts;
using Nethereum.RPC.NonceServices;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Signer;

namespace Nethereum.Web3.Accounts
{
    public class Account : IAccount
    {
        public BigInteger? ChainId { get; private set; }

#if !PCL
        public static Account LoadFromKeyStoreFile(string filePath, string password)
        {
            var keyStoreService = new Nethereum.KeyStore.KeyStoreService();
            var key = keyStoreService.DecryptKeyStoreFromFile(password, filePath);
            return new Account(key);
        }
#endif
        public static Account LoadFromKeyStore(string json, string password, BigInteger? chainId = null)
        {
            var keyStoreService = new KeyStoreService();
            var key = keyStoreService.DecryptKeyStoreFromJson(password, json);
            return new Account(key, chainId);
        }

        public string PrivateKey { get; private set; }

        public Account(EthECKey key, BigInteger? chainId = null)
        {
            ChainId = chainId;
            Initialise(key);
        }

        public Account(string privateKey, BigInteger? chainId = null)
        {
            ChainId = chainId;
            Initialise(new EthECKey(privateKey));
        }

        public Account(byte[] privateKey, BigInteger? chainId = null)
        {
            ChainId = chainId;
            Initialise(new EthECKey(privateKey, true));
        }

        public Account(EthECKey key, ChainId chainId):this(key, (int)chainId)
        {
            
        }

        public Account(string privateKey, ChainId chainId) : this(privateKey, (int)chainId)
        {
            
        }

        public Account(byte[] privateKey, ChainId chainId) : this(privateKey, (int)chainId)
        {

        }

        private void Initialise(EthECKey key)
        {
            PrivateKey = key.GetPrivateKey();
            Address = key.GetPublicAddress();
            InitialiseDefaultTransactionManager();
        }

        protected virtual void InitialiseDefaultTransactionManager()
        {
            TransactionManager = new AccountSignerTransactionManager(null, this, ChainId);
        }

        public string Address { get; protected set; }
        public ITransactionManager TransactionManager { get; protected set; }
        public INonceService NonceService { get; set; }
    }
}
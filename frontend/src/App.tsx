import React, { useState, useRef } from 'react';
import { documentApi, ragApi, SearchRequest, SearchResult, UploadResponse, RagSearchRequest, RagSearchResponse } from './services/api';
import './App.css';

const App: React.FC = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<SearchResult[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadMessage, setUploadMessage] = useState('');
  const [error, setError] = useState('');
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [searchMode, setSearchMode] = useState<'vector' | 'rag'>('vector');
  const [ragResponse, setRagResponse] = useState<RagSearchResponse | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file && file.type === 'application/pdf') {
      setSelectedFile(file);
      setError('');
    } else {
      setError('Please select a valid PDF file');
      setSelectedFile(null);
    }
  };

  const handleDrop = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault();
    const file = event.dataTransfer.files[0];
    if (file && file.type === 'application/pdf') {
      setSelectedFile(file);
      setError('');
    } else {
      setError('Please drop a valid PDF file');
    }
  };

  const handleDragOver = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault();
  };

  const handleUpload = async () => {
    if (!selectedFile) {
      setError('Please select a PDF file first');
      return;
    }

    setIsUploading(true);
    setError('');
    setUploadMessage('');

    try {
      const response: UploadResponse = await documentApi.uploadPdf(selectedFile);
      if (response.success) {
        setUploadMessage(`Successfully processed ${response.chunksProcessed} chunks from ${selectedFile.name}`);
        setSelectedFile(null);
        if (fileInputRef.current) {
          fileInputRef.current.value = '';
        }
      } else {
        setError(response.message);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to upload PDF');
    } finally {
      setIsUploading(false);
    }
  };

  const handleSearch = async (event: React.FormEvent) => {
    event.preventDefault();
    
    if (!searchQuery.trim()) {
      setError('Please enter a search query');
      return;
    }

    setIsSearching(true);
    setError('');
    setSearchResults([]);
    setRagResponse(null);

    try {
      if (searchMode === 'vector') {
        const searchRequest: SearchRequest = {
          query: searchQuery.trim(),
          limit: 10,
          threshold: 0.3
        };

        const results = await documentApi.search(searchRequest);
        setSearchResults(results);
        
        if (results.length === 0) {
          setError('No results found for your query');
        }
      } else {
        const ragRequest: RagSearchRequest = {
          query: searchQuery.trim(),
          maxResults: 5,
          threshold: 0.3,
          includeSourceDocuments: true
        };

        const response = await ragApi.searchWithRag(ragRequest);
        
        if (response.success) {
          setRagResponse(response);
          setSearchResults(response.sourceDocuments || []);
        } else {
          setError(response.errorMessage || 'RAG search failed');
        }
      }
    } catch (err: any) {
      setError(err.response?.data?.message || err.response?.data || `Failed to ${searchMode} search documents`);
    } finally {
      setIsSearching(false);
    }
  };

  const clearResults = () => {
    setSearchResults([]);
    setSearchQuery('');
    setError('');
    setRagResponse(null);
  };

  const handleModeChange = (mode: 'vector' | 'rag') => {
    setSearchMode(mode);
    clearResults();
  };

  return (
    <div className="container">
      <div className="header">
        <h1>PDF Semantic Search</h1>
        <p>Upload PDF documents and search them using AI-powered semantic search</p>
      </div>

      {/* Upload Section */}
      <div className="upload-section">
        <h2>Upload PDF Document</h2>
        <div 
          className={`upload-area ${selectedFile ? 'has-file' : ''}`}
          onDrop={handleDrop}
          onDragOver={handleDragOver}
          onClick={() => fileInputRef.current?.click()}
        >
          <input
            ref={fileInputRef}
            type="file"
            accept=".pdf"
            onChange={handleFileSelect}
            className="file-input"
          />
          {selectedFile ? (
            <div className="file-selected">
              <p>📄 {selectedFile.name}</p>
              <p className="file-size">Size: {(selectedFile.size / 1024 / 1024).toFixed(2)} MB</p>
            </div>
          ) : (
            <div className="upload-prompt">
              <p>📁 Click to select a PDF file or drag and drop here</p>
              <p className="upload-hint">Only PDF files are supported</p>
            </div>
          )}
        </div>
        
        <button 
          className="upload-button"
          onClick={handleUpload}
          disabled={!selectedFile || isUploading}
        >
          {isUploading ? 'Processing...' : 'Upload & Process PDF'}
        </button>

        {uploadMessage && (
          <div className="success">
            ✅ {uploadMessage}
          </div>
        )}
      </div>

      {/* Search Section */}
      <div className="search-section">
        <h2>Search Documents</h2>
        
        {/* Search Mode Toggle */}
        <div className="search-mode-toggle">
          <button 
            type="button"
            className={`mode-button ${searchMode === 'vector' ? 'active' : ''}`}
            onClick={() => handleModeChange('vector')}
            disabled={isSearching}
          >
            📊 Vector Search
          </button>
          <button 
            type="button"
            className={`mode-button ${searchMode === 'rag' ? 'active' : ''}`}
            onClick={() => handleModeChange('rag')}
            disabled={isSearching}
          >
            🤖 RAG Search
          </button>
        </div>
        
        <div className="search-mode-description">
          {searchMode === 'vector' ? (
            <p>🔍 Find semantically similar content in your documents</p>
          ) : (
            <p>🤖 Get AI-generated answers based on your document content</p>
          )}
        </div>
        
        <form onSubmit={handleSearch} className="search-form">
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder={searchMode === 'vector' ? "Enter your search query..." : "Ask a question about your documents..."}
            className="search-input"
            disabled={isSearching}
          />
          <button 
            type="submit" 
            className="search-button"
            disabled={isSearching || !searchQuery.trim()}
          >
            {isSearching ? 'Searching...' : searchMode === 'vector' ? '🔍 Search' : '🤖 Ask AI'}
          </button>
          {(searchResults.length > 0 || ragResponse) && (
            <button 
              type="button" 
              className="clear-button"
              onClick={clearResults}
            >
              Clear
            </button>
          )}
        </form>

        {error && (
          <div className="error">
            ❌ {error}
          </div>
        )}
      </div>

      {/* RAG Answer Section */}
      {ragResponse && ragResponse.generatedAnswer && (
        <div className="rag-answer-section">
          <h2>🤖 AI Generated Answer</h2>
          <div className="rag-answer">
            <div className="answer-content">
              {ragResponse.generatedAnswer}
            </div>
            <div className="answer-metadata">
              <span className="tokens-used">Tokens: {ragResponse.tokensUsed}</span>
              <span className="response-time">Time: {ragResponse.responseTimeMs}ms</span>
            </div>
          </div>
        </div>
      )}

      {/* Results Section */}
      {(searchResults.length > 0 || ragResponse || isSearching) && (
        <div className="results-section">
          <h2>{searchMode === 'rag' ? '📚 Source Documents' : '🔍 Search Results'}</h2>
          
          {isSearching ? (
            <div className="loading">
              <p>🔄 {searchMode === 'rag' ? 'Generating AI answer...' : 'Searching documents...'}</p>
            </div>
          ) : searchResults.length > 0 ? (
            <>
              <p className="results-count">
                {searchMode === 'rag' 
                  ? `Based on ${searchResults.length} source document${searchResults.length > 1 ? 's' : ''}` 
                  : `Found ${searchResults.length} relevant result${searchResults.length > 1 ? 's' : ''}`
                }
              </p>
              {searchResults.map((result, index) => (
                <div key={result.id} className="result-item">
                  <div className="result-header">
                    <span className="result-filename">📄 {result.fileName}</span>
                    <span className="result-score">
                      Score: {(result.score * 100).toFixed(1)}%
                    </span>
                  </div>
                  <div className="result-page">
                    📍 Page {result.pageNumber}
                  </div>
                  <div className="result-content">
                    {result.content}
                  </div>
                </div>
              ))}
            </>
          ) : (
            <div className="no-results">
              <p>No results found. Try a different search query.</p>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default App;

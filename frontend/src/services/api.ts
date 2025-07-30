import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export interface SearchRequest {
  query: string;
  limit: number;
  threshold: number;
}

export interface SearchResult {
  id: string;
  content: string;
  fileName: string;
  pageNumber: number;
  score: number;
}

export interface UploadResponse {
  success: boolean;
  message: string;
  chunksProcessed: number;
}

export interface RagSearchRequest {
  query: string;
  maxResults: number;
  threshold: number;
  includeSourceDocuments: boolean;
}

export interface RagSearchResponse {
  generatedAnswer: string;
  sourceDocuments: SearchResult[];
  query: string;
  tokensUsed: number;
  responseTimeMs: number;
  success: boolean;
  errorMessage: string;
}

export const documentApi = {
  uploadPdf: async (file: File): Promise<UploadResponse> => {
    const formData = new FormData();
    formData.append('file', file);
    
    const response = await api.post('/document/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    
    return response.data;
  },

  search: async (searchRequest: SearchRequest): Promise<SearchResult[]> => {
    const response = await api.post('/document/search', searchRequest);
    return response.data;
  },

  health: async (): Promise<{ status: string; timestamp: string }> => {
    const response = await api.get('/document/health');
    return response.data;
  },
};

export const ragApi = {
  searchWithRag: async (ragRequest: RagSearchRequest): Promise<RagSearchResponse> => {
    const response = await api.post('/rag/search', ragRequest);
    return response.data;
  },

  health: async (): Promise<{ status: string; service: string; timestamp: string }> => {
    const response = await api.get('/rag/health');
    return response.data;
  },
};

export default api;

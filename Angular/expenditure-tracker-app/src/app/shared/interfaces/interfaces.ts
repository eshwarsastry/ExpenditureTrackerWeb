
/*
Interface for the Refresh Token (can look different, based on your backend api)
*/
export interface RefreshToken {
  id: number;
  userId: number;
  token: string;
  refreshCount: number;
  expiryDate: Date;
}

/*
Interface for the Login Response (can look different, based on your backend api)
*/
export interface LoginResponse {
  accessToken: string;
  refreshToken: RefreshToken;
  tokenType: string;
  userId: number;
  userName: string;
  responseCode: number;
  message: string;
}

/*
Interface for the Login Request (can look different, based on your backend api)
*/
export interface LoginRequest {
  email: string;
  password: string;
}

/*
Interface for the Register Request (can look different, based on your backend api)
*/
export interface RegisterRequest {
  email: string;
  username: string;
  firstname: string;
  lastname: string;
  password: string;
}

/*
Interface for the Register Response (can look different, based on your backend api)
*/
export interface RegisterResponse {
  responseCode: number;
  message: string;
}

/*
Interface for the Transaction Categories (can look different, based on your backend api)
*/
export interface TransactionCategory {
  id?: number;
  user_Id: number;
  transactionType_Id: number;
  name: string;
  description: string;
}

/*
Interface for the Transaction (can look different, based on your backend api)
*/
export interface Transactions {
  id: number;
  user_Id: number;
  category_Id: number;
  amount: number;
  transactionDate: Date;
  note: string;
}

/*
Interface for the Transaction Type (can look different, based on your backend api)
*/
export interface TransactionType {
  id: number;
  transactionType: string;
}


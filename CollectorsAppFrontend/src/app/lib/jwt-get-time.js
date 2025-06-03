import { jwtDecode } from 'jwt-decode';

const EXPIRATION_BUFFER_SECONDS = 10;

export function getTokenExpirationDate(token) {

  if (!token) {
    return true
  }

  const decoded = jwtDecode(token);

  if (!decoded.exp) {
    return null;
  }

  const expirationDate = new Date(0);
  expirationDate.setUTCSeconds(decoded.exp);

  return expirationDate;
}

export function isTokenExpired(token) {
  try {
    const expirationDate = getTokenExpirationDate(token);
    if (!expirationDate) {
      return false;
    }
    const bufferedNow = new Date(Date.now() + EXPIRATION_BUFFER_SECONDS * 1000);

    return expirationDate < bufferedNow;
  } catch (e) {
    return true;
  }
}

export function getSecondsUntilExpiration(token) {
  const expirationDate = getTokenExpirationDate(token);
  if (!expirationDate) {
    return null;
  }
  const now = new Date();
  const diffMs = expirationDate - now;
  return Math.floor(diffMs / 1000);
}

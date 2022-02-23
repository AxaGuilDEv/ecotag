export const fetchGroups = fetch => async () => fetch('groups');

export const fetchGroup = fetch => async id => fetch(`groups/${id}`);

export const fetchUsers = fetch => async () => fetch('users');

export const fetchDeleteGroup = fetch => async id =>
  fetch(`groups/${id}`, {
    method: 'DELETE',
  });

export const fetchCreateGroup = fetch => async newGroup =>
  fetch('groups', {
      method: 'POST',
      body: JSON.stringify(newGroup),
  });

export const fetchUpdateGroup = fetch => async newGroup =>
  fetch('groups', {
      method: 'PUT',
      body: JSON.stringify(newGroup),
  });

export const fetchCreateOrUpdateGroup = fetch => async newGroup =>
  fetch('groups', {
    method: 'POST',
    body: JSON.stringify(newGroup),
  });

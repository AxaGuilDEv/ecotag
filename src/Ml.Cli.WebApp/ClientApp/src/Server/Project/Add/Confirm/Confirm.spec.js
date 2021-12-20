import '@testing-library/jest-dom/extend-expect';
import React from 'react';
import { render, waitForElement } from '@testing-library/react';
import {ConfirmContainer} from './Confirm';
import routeData from 'react-router';
import {BrowserRouter as Router} from "react-router-dom";

describe('ConfirmContainer', () => {
  const mockLocation = {
    pathname: '/projects/confirm',
    hash: '',
    search: '',
    state: {
      project: {
        id: "projectId"
      }
    },
  };
  beforeEach(() => {
    jest.spyOn(routeData, 'useLocation').mockReturnValue(mockLocation);
  });

  afterEach(() => {
    jest.clearAllMocks();
  });
  it('ConfirmContainer render correctly', async () => {
    const { getByText } = render(<Router><ConfirmContainer history={[]} /></Router>);
    const messageEl = await waitForElement(() => getByText('Nouveau projet ajouté !'));
    expect(messageEl).toHaveTextContent(
        'Nouveau projet ajouté !'
    );
  });
});


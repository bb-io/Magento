# Blackbird.io Magento

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

Magento is the leading platform for open commerce innovation. It’s designed to be flexible and scalable, able to support businesses of all sizes – from small startups to large enterprises.

<!-- begin docs -->

## Before setting up

- You can find the `Base URL` of your Magento URL. It can look like `https://magento.testing.organization.com/`
- For receiving the `Consumer key`, `Consumer secret`, `Access token` and `Access token secret` you need to create a new integration in your Magento account. You can do this by going to `System` -> `Extensions` -> `Integrations` -> `Add new integration`. After creating the integration you will receive the `Consumer key`, `Consumer secret`, `Access token` and `Access token secret`. Note, that you don't need to provide any values for the `Callback URL` and `Identity link URL` fields. You can select all the available resources for the integration to avoid any issues with the permissions. Or select the resources that you need for your specific use case. See the current available functionality of the app below in the documentation.

## Connecting

1. Navigate to Apps, and identify the **Magento** app. You can use search to find it.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My organization'.
4. Fill all the required fields:
    - `Base URL` 
    - `Consumer key`
    - `Consumer secret`
    - `Access token`
    - `Access token secret`
5. Click _Connect_.
6. Confirm that the connection has appeared and the status is _Connected_.

![Connection](image/README/connection.png)

## Actions

### Blocks

- **Get all blocks** - Get all blocks.
- **Get block** - Get block by specified ID.
- **Get block as HTML** - Get block by specified ID as HTML.
- **Create block** - Create block with specified data.
- **Update block** - Update block with specified data.
- **Update block from HTML** - Update block with specified ID from HTML. This action is useful in pair with the `Get block as HTML` action when you need to get the block content as HTML and then translate the document.
- **Delete block** - Delete block by specified ID.

### Pages

- **Get all pages** - Get all pages.
- **Get page** - Get page by specified ID.
- **Get page as HTML** - Get page by specified ID as HTML.
- **Create page** - Create page with specified data.
- **Update page** - Update page with specified data.
- **Update page from HTML** - Update page with specified ID from HTML. This action is useful in pair with the `Get page as HTML` action when you need to get the page content as HTML and then translate the document.
- **Delete page** - Delete page by specified ID.

### Products

- **Get all products** - Get all products.
- **Get product** - Get product by specified SKU.
- **Get product as HTML** - Get product by specified SKU as HTML. Using optional parameters you can specify `Custom attributes` that you want to include in the HTML.
- **Create product** - Create product with specified data.
- **Update product** - Update product with specified data.
- **Update product from HTML** - Update product with specified SKU from HTML. This action is useful in pair with the `Get product as HTML` action when you need to get the product content as HTML and then translate the document.
- **Delete product** - Delete product by specified SKU.
- **Add custom attribute** - Add custom attribute to product by specified SKU.

## Events

- **On products created** - Triggered when new products are created.
- **On products updated** - Triggered when products are updated.
- **On blocks created** - Triggered when new blocks are created.
- **On blocks updated** - Triggered when blocks are updated.
- **On pages created** - Triggered when new pages are created.
- **On pages updated** - Triggered when pages are updated.

Note, that these events are based on polling mechanism and are triggered periodically. The interval can be set in the bird editor. The interval should be between 5 minutes and 7 days.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->

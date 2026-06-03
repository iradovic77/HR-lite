import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Table,
  Button,
  Tag,
  Space,
  Modal,
  Form,
  Input,
  InputNumber,
  Switch,
  Popconfirm,
  Typography,
  Card,
} from 'antd'
import { PlusOutlined, EditOutlined, StopOutlined, CheckOutlined } from '@ant-design/icons'
import type { ColumnsType } from 'antd/es/table'
import type { GenderItem } from '@/types/codebook'

const { Title } = Typography

// ── Mock podaci (zamijenit će se API pozivima) ─────────────────────────
const INITIAL_DATA: GenderItem[] = [
  { id: '1', code: 'M', nameHr: 'Muško',  nameEn: 'Male',   ordinal: 1, isActive: true },
  { id: '2', code: 'F', nameHr: 'Žensko', nameEn: 'Female', ordinal: 2, isActive: true },
  { id: '3', code: 'O', nameHr: 'Ostalo', nameEn: 'Other',  ordinal: 3, isActive: true },
]

export default function GenderPage() {
  const { t } = useTranslation()
  const [data, setData] = useState<GenderItem[]>(INITIAL_DATA)
  const [modalOpen, setModalOpen] = useState(false)
  const [editingItem, setEditingItem] = useState<GenderItem | null>(null)
  const [form] = Form.useForm<Omit<GenderItem, 'id'>>()

  // ── Handlers ────────────────────────────────────────────────────────

  const openAddModal = () => {
    setEditingItem(null)
    form.resetFields()
    form.setFieldsValue({ isActive: true, ordinal: data.length + 1 })
    setModalOpen(true)
  }

  const openEditModal = (item: GenderItem) => {
    setEditingItem(item)
    form.setFieldsValue(item)
    setModalOpen(true)
  }

  const handleSave = () => {
    form.validateFields().then((values) => {
      if (editingItem) {
        // Edit — zamijeni u listi
        setData((prev) =>
          prev.map((item) =>
            item.id === editingItem.id ? { ...item, ...values } : item
          )
        )
      } else {
        // Add — dodaj novi zapis s privremenim ID-em
        const newItem: GenderItem = {
          ...values,
          id: `temp-${Date.now()}`,
        }
        setData((prev) => [...prev, newItem])
      }
      setModalOpen(false)
    })
  }

  const toggleActive = (item: GenderItem) => {
    setData((prev) =>
      prev.map((g) =>
        g.id === item.id ? { ...g, isActive: !g.isActive } : g
      )
    )
  }

  // ── Kolone tablice ──────────────────────────────────────────────────

  const columns: ColumnsType<GenderItem> = [
    {
      title: t('codebook.gender.columns.code'),
      dataIndex: 'code',
      key: 'code',
      width: 80,
      sorter: (a, b) => a.code.localeCompare(b.code),
    },
    {
      title: t('codebook.gender.columns.nameHr'),
      dataIndex: 'nameHr',
      key: 'nameHr',
    },
    {
      title: t('codebook.gender.columns.nameEn'),
      dataIndex: 'nameEn',
      key: 'nameEn',
    },
    {
      title: t('codebook.gender.columns.ordinal'),
      dataIndex: 'ordinal',
      key: 'ordinal',
      width: 110,
      sorter: (a, b) => a.ordinal - b.ordinal,
      defaultSortOrder: 'ascend',
    },
    {
      title: t('codebook.gender.columns.status'),
      dataIndex: 'isActive',
      key: 'isActive',
      width: 110,
      render: (isActive: boolean) =>
        isActive
          ? <Tag color="success">{t('codebook.gender.status.active')}</Tag>
          : <Tag color="default">{t('codebook.gender.status.inactive')}</Tag>,
    },
    {
      title: t('codebook.gender.columns.actions'),
      key: 'actions',
      width: 160,
      render: (_, record) => (
        <Space>
          <Button
            type="text"
            icon={<EditOutlined />}
            size="small"
            onClick={() => openEditModal(record)}
          >
            {t('codebook.gender.actions.edit')}
          </Button>

          <Popconfirm
            title={t(
              record.isActive
                ? 'codebook.gender.confirm.deactivate'
                : 'codebook.gender.confirm.activate'
            )}
            onConfirm={() => toggleActive(record)}
            okText={t('common.yes')}
            cancelText={t('common.no')}
          >
            <Button
              type="text"
              icon={record.isActive ? <StopOutlined /> : <CheckOutlined />}
              size="small"
              danger={record.isActive}
            >
              {t(
                record.isActive
                  ? 'codebook.gender.actions.deactivate'
                  : 'codebook.gender.actions.activate'
              )}
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ]

  // ── Render ──────────────────────────────────────────────────────────

  return (
    <Card>
      {/* Header kartice: naslov + gumb Dodaj novi */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <Title level={4} style={{ margin: 0 }}>
          {t('codebook.gender.title')}
        </Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={openAddModal}>
          {t('codebook.gender.addNew')}
        </Button>
      </div>

      {/* Tablica */}
      <Table
        columns={columns}
        dataSource={data}
        rowKey="id"
        size="small"
        pagination={{ pageSize: 20, showSizeChanger: false }}
      />

      {/* Modal forma za dodavanje / uređivanje */}
      <Modal
        title={editingItem
          ? t('codebook.gender.modal.editTitle')
          : t('codebook.gender.modal.addTitle')
        }
        open={modalOpen}
        onOk={handleSave}
        onCancel={() => setModalOpen(false)}
        okText={t('common.save')}
        cancelText={t('common.cancel')}
        destroyOnClose
      >
        <Form
          form={form}
          layout="vertical"
          style={{ marginTop: 16 }}
        >
          <Form.Item
            name="code"
            label={t('codebook.gender.modal.code')}
            tooltip={t('codebook.gender.modal.code_hint')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={10} style={{ textTransform: 'uppercase' }} />
          </Form.Item>

          <Form.Item
            name="nameHr"
            label={t('codebook.gender.modal.nameHr')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={100} />
          </Form.Item>

          <Form.Item
            name="nameEn"
            label={t('codebook.gender.modal.nameEn')}
          >
            <Input maxLength={100} />
          </Form.Item>

          <Form.Item
            name="ordinal"
            label={t('codebook.gender.modal.ordinal')}
          >
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item
            name="isActive"
            label={t('codebook.gender.modal.isActive')}
            valuePropName="checked"
          >
            <Switch />
          </Form.Item>
        </Form>
      </Modal>
    </Card>
  )
}
